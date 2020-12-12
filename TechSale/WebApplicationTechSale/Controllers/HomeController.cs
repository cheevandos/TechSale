using DataAccessLogic.DatabaseModels;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TechSaleTelegramBot;

namespace WebApplicationTechSale.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBot bot;
        private readonly ICrudLogic<User> userLogic;

        public HomeController(IBot bot, ICrudLogic<User> userLogic)
        {
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
    }
}
