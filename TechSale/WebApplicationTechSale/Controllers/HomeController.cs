﻿using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechSaleTelegramBot;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBot bot;
        private readonly ICrudLogic<User> userLogic;
        private readonly IPagination<AuctionLot> lotLogic;

        public HomeController(IPagination<AuctionLot> lotLogic, IBot bot, ICrudLogic<User> userLogic)
        {
            this.lotLogic = lotLogic;
            this.bot = bot;
            this.userLogic = userLogic;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult SendMessageToBot(string msg)
        {
            User user = userLogic.Read(new User
            {
                UserName = User.Identity.Name
            })?.First();

            if (user != null && !string.IsNullOrWhiteSpace(user.TelegramChatId))
            {
                bot.SendMessage(msg, user.TelegramChatId);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Lots(int page = 1)
        {
            List<AuctionLot> lotsToDisplay = await lotLogic.GetPage(page);
            int lotsCount = await lotLogic.GetCount();

            return View(new AuctionLotsViewModel()
            {
                PageViewModel = new PageViewModel(lotsCount, page, ApplicationConstantsProvider.GetPageSize()),
                AuctionLots = lotsToDisplay
            });
        }
    }
}
