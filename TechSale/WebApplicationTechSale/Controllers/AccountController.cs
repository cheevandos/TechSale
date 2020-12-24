using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechSaleTelegramBot;
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
        private readonly IBot telegramBot;

        public AccountController(IPagination<AuctionLot> lotLogic, ISavedLogic savedListLogic,
            UserManager<User> userManager, SignInManager<User> signInManager, IBot telegramBot)
        {
            this.lotLogic = lotLogic;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.savedListLogic = savedListLogic;
            this.telegramBot = telegramBot;
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.OldPassword == model.NewPassword)
                {
                    ModelState.AddModelError(string.Empty, "Новый и старый пароли не должны совпадать");
                    return View(model);
                }

                User user = await userManager.FindByNameAsync(User.Identity.Name);

                user.UserName += ApplicationConstantsProvider.AvoidValidationCode();
                user.Email += ApplicationConstantsProvider.AvoidValidationCode();

                var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (changePasswordResult.Succeeded)
                {
                    return View("Redirect", new RedirectModel
                    {
                        InfoMessages = RedirectionMessageProvider.AccountUpdatedMessages(),
                        RedirectUrl = "/Account/Personal",
                        SecondsToRedirect = ApplicationConstantsProvider.GetShortRedirectionTime()
                    });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Вы ввели неверный старый пароль");
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
                UserName = user.UserName,
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
                Microsoft.AspNetCore.Identity.SignInResult loginResult;
                User user = await userManager.FindByEmailAsync(model.Login);
                if (user != null)
                {
                    loginResult = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                }
                else
                {
                    user = await userManager.FindByNameAsync(model.Login);
                    loginResult = await signInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, false);
                }

                if (loginResult.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {

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
                    UserName = model.UserName,
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

        [Authorize(Roles = "regular user")]
        [HttpGet]
        public IActionResult Update()
        {
            return View();
        }

        [Authorize(Roles = "regular user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                User userToUpdate = await userManager.FindByNameAsync(User.Identity.Name);
                if (model.NewEmail == userToUpdate.Email)
                {
                    ModelState.AddModelError(string.Empty, "Новый email не должен совпадать со старыми");
                }
                else
                {

                    if (string.IsNullOrWhiteSpace(model.NewTelegramUserName))
                    {
                        userToUpdate.TelegramUsername = string.Empty;

                        if (!string.IsNullOrWhiteSpace(userToUpdate.TelegramChatId))
                        {
                            await telegramBot.SendMessage("Вы отписались от уведомлений через сайт", userToUpdate.TelegramChatId);
                            userToUpdate.TelegramChatId = string.Empty;
                        }
                    }

                    userToUpdate.UserName += ApplicationConstantsProvider.AvoidValidationCode();
                    var updateEmailResult = await userManager.SetEmailAsync(userToUpdate, model.NewEmail);

                    if (updateEmailResult.Succeeded)
                    {
                        return View("Redirect", new RedirectModel
                        {
                            InfoMessages = RedirectionMessageProvider.AccountUpdatedMessages(),
                            RedirectUrl = "/Account/Personal",
                            SecondsToRedirect = ApplicationConstantsProvider.GetShortRedirectionTime()
                        });
                    }
                    else
                    {
                        foreach (var emailUpdateError in updateEmailResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, emailUpdateError.Description);
                        }
                    }
                }
            }
            return View(model);
        }
    }
}
