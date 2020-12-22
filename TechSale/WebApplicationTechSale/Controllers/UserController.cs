using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationTechSale.HelperServices;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    [Authorize(Roles = "regular user")]
    public class UserController : Controller
    {
        private readonly ICrudLogic<AuctionLot> lotLogic;
        private readonly IWebHostEnvironment environment;

        public UserController(ICrudLogic<AuctionLot> lotLogic, IWebHostEnvironment environment)
        {
            this.lotLogic = lotLogic;
            this.environment = environment;
        }

        [HttpGet]
        public IActionResult CreateLot()
        {
            return View();
        }

        [HttpPost]
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

                if (lot == null)
                {
                    return NotFound();
                }

                return View(lot);
            }
            return NotFound();
        }
    }
}
