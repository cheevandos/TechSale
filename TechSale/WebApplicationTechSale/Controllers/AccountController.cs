using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationTechSale.HelperServices;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IPagination<AuctionLot> lotLogic;
        private readonly ISavedLogic savedListLogic;

        public AccountController(IPagination<AuctionLot> lotLogic, ISavedLogic savedListLogic,
            UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.lotLogic = lotLogic;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.savedListLogic = savedListLogic;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            User user = await userManager.FindByIdAsync(userManager.FindByNameAsync(User.Identity.Name).Result.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                Id = user.Id,
                Email = user.Email
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(userManager, user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                        await userManager.UpdateAsync(user);
                        return RedirectToAction("Personal");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Personal(int page = 1)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);

            List<AuctionLot> userLots = new List<AuctionLot>();
            int count = 0;

            if (await userManager.IsInRoleAsync(user, "regular user"))
            {
                userLots = await lotLogic.GetPage(page, new AuctionLot
                {
                    User = user
                });
                count = await lotLogic.GetCount(new AuctionLot
                {
                    User = user
                });
            }

            PersonalAccountViewModel model = new PersonalAccountViewModel
            {
                Email = user.Email,
                TelegramId = user.TelegramUsername,
                PersonalLotsList = new AuctionLotsViewModel
                {
                    AuctionLots = userLots,
                    PageViewModel = new PageViewModel(count, page, ApplicationConstantsProvider.GetPageSize())
                }
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Lots", "Home");
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, false);
                if (loginResult.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        User user = await userManager.FindByEmailAsync(model.Email);
                        if (await userManager.IsInRoleAsync(user, "admin"))
                        {
                            return RedirectToAction("UsersList", "Admin");
                        }
                        if (await userManager.IsInRoleAsync(user, "moderator"))
                        {
                            return RedirectToAction("Lots", "Moderator");
                        }
                        if (await userManager.IsInRoleAsync(user, "regular user"))
                        {
                            return RedirectToAction("Lots", "Home");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Lots", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Lots", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Email,
                    TelegramUsername = string.IsNullOrWhiteSpace(model.TelegramId) ?
                    string.Empty : model.TelegramId
                };
                var registerResult = await userManager.CreateAsync(user, model.Password);
                if (registerResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "regular user");
                    await savedListLogic.Create(user);
                    await signInManager.SignInAsync(user, false);
                    return View("Redirect", new RedirectModel
                    {
                        InfoMessages = RedirectionMessageProvider.AccountCreatedMessages(),
                        RedirectUrl = "/Home/Lots",
                        SecondsToRedirect = ApplicationConstantsProvider.GetShortRedirectionTime()
                    });
                }
                else
                {
                    foreach (var error in registerResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}
