using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechSaleTelegramBot;
using WebApplicationTechSale.HelperServices;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    [Authorize(Roles = "regular user")]
    public class UserController : Controller
    {
        private readonly ICrudLogic<AuctionLot> lotLogic;
        private readonly ICrudLogic<Bid> bidLogic;
        private readonly IWebHostEnvironment environment;
        private readonly ISavedLogic savedListLogic;
        private readonly UserManager<User> userManager;
        private readonly IBot telegramBot;

        public UserController(ICrudLogic<AuctionLot> lotLogic, IWebHostEnvironment environment,
            UserManager<User> userManager, ICrudLogic<Bid> bidLogic, ISavedLogic savedListLogic,
            IBot telegramBot)
        {
            this.lotLogic = lotLogic;
            this.environment = environment;
            this.userManager = userManager;
            this.bidLogic = bidLogic;
            this.savedListLogic = savedListLogic;
            this.telegramBot = telegramBot;
        }

        [HttpGet]
        public IActionResult CreateLot()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLot(CreateLotViewModel model)
        {
            if (ModelState.IsValid)
            {
                AuctionLot toAdd = new AuctionLot
                {
                    Name = model.Name,
                    User = new User
                    {
                        UserName = User.Identity.Name
                    },
                    Description = model.Description,
                    StartDate = model.StartDate.Value,
                    EndDate = model.EndDate.Value,
                    PriceInfo = new PriceInfo
                    {
                        StartPrice = model.StartPrice.Value,
                        CurrentPrice = model.StartPrice.Value,
                        BidStep = model.BidStep.Value
                    }
                };

                string path = $"/images/{User.Identity.Name}/{model.Name}";

                if (!Directory.Exists($"{environment.WebRootPath + path}"))
                {
                    Directory.CreateDirectory($"{environment.WebRootPath + path}");
                }

                path += $"/photo{Path.GetExtension(model.Photo.FileName)}";

                using (FileStream fs = new FileStream($"{environment.WebRootPath + path}", FileMode.Create))
                {
                    await model.Photo.CopyToAsync(fs);
                }

                toAdd.PhotoSrc = path;

                await lotLogic.Create(toAdd);

                return View("Redirect", new RedirectModel
                {
                    InfoMessages = RedirectionMessageProvider.LotCreatedMessages(),
                    RedirectUrl = "/Home/Lots",
                    SecondsToRedirect = ApplicationConstantsProvider.GetMaxRedirectionTime()
                });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> OpenLot(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                AuctionLot lot = (await lotLogic.Read(new AuctionLot
                {
                    Id = id
                })).First();

                lot.Bids = await bidLogic.Read(new Bid
                {
                    AuctionLotId = id
                });

                User user = await userManager.FindByNameAsync(User.Identity.Name);

                SavedList userList = await savedListLogic.Read(user);

                if (userList.AuctionLots.Any(lot => lot.Id == id))
                {
                    ViewBag.IsSaved = true;
                }
                else
                {
                    ViewBag.IsSaved = false;
                }

                if (lot == null)
                {
                    return NotFound();
                }
                return View(lot);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceBid(string lotId)
        {
            if (!string.IsNullOrWhiteSpace(lotId))
            {
                await bidLogic.Create(new Bid
                {
                    AuctionLot = (await lotLogic.Read(new AuctionLot
                    {
                        Id = lotId
                    }))?.First(),
                    User = await userManager.FindByNameAsync(User.Identity.Name)
                });
                await SendNotifications(lotId, User.Identity.Name);
                return View("Redirect", new RedirectModel
                {
                    InfoMessages = RedirectionMessageProvider.BidPlacedMessages(),
                    RedirectUrl = $"/User/OpenLot/?id={lotId}",
                    SecondsToRedirect = ApplicationConstantsProvider.GetShortRedirectionTime()
                });
            }
            return NotFound();
        }

        private async Task SendNotifications(string lotId, string userName)
        {
            AuctionLot auctionLot = (await lotLogic.Read(new AuctionLot 
            { 
                Id = lotId 
            })).First();

            List<Bid> bids = await bidLogic.Read(new Bid
            {
                AuctionLotId = lotId
            });


            List<User> users = new List<User>();

            foreach (Bid bid in bids)
            {
                if (!string.IsNullOrWhiteSpace(bid.User.TelegramChatId)
                    &&!users.Contains(bid.User) 
                    && userName != bid.User.UserName)
                {
                    users.Add(bid.User);
                }
            }

            foreach (User user in users)
            {
                await telegramBot.SendMessage(
                    $"Новая ставка в лоте '{auctionLot.Name}', " +
                    $"текущая цена {auctionLot.PriceInfo.CurrentPrice}", 
                    user.TelegramChatId);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLot(string lotId)
        {
            if (!string.IsNullOrWhiteSpace(lotId))
            {
                User user = await userManager.FindByNameAsync(User.Identity.Name);
                AuctionLot lotToAdd = new AuctionLot { Id = lotId };
                await savedListLogic.Add(user, lotToAdd);
                return RedirectToAction("OpenLot", "User", new { id = lotId });
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> MySavedList()
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);

            SavedList userSavedList = await savedListLogic.Read(user);

            return View(userSavedList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLot(string lotId)
        {
            if (!string.IsNullOrWhiteSpace(lotId))
            {
                User user = await userManager.FindByNameAsync(User.Identity.Name);
                AuctionLot lotToAdd = new AuctionLot { Id = lotId };
                await savedListLogic.Remove(user, lotToAdd);
                return RedirectToAction("OpenLot", "User", new { id = lotId });
            }
            return NotFound();
        }
    }
}
