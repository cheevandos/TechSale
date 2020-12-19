using DataAccessLogic.DatabaseModels;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    [Authorize(Roles = "regular user")]
    public class SellerController : Controller
    {
        private readonly ICrudLogic<AuctionLot> lotLogic;
        private readonly IWebHostEnvironment environment;

        public SellerController(ICrudLogic<AuctionLot> lotLogic, IWebHostEnvironment environment)
        {
            this.lotLogic = lotLogic;
            this.environment = environment;
        }

        [HttpGet]
        public IActionResult ListLots()
        {
            List<AuctionLot> userLots = lotLogic.Read(new AuctionLot
            {
                User = new User
                {
                    UserName = User.Identity.Name
                }
            });
            return View(userLots);
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
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    PriceInfo = new PriceInfo
                    {
                        StartPrice = model.StartPrice,
                        CurrentPrice = model.StartPrice,
                        BidStep = model.BidStep
                    }
                };

                string path = $"/images/{User.Identity.Name}/{model.Name}";

                if (!Directory.Exists($"{environment.WebRootPath + path}"))
                {
                    Directory.CreateDirectory($"{environment.WebRootPath + path}");
                }

                path += $"/{model.Photo.FileName}";

                using (FileStream fs = new FileStream($"{environment.WebRootPath + path}", FileMode.Create))
                {
                    await model.Photo.CopyToAsync(fs);
                }

                toAdd.PhotoSrc = path;

                await lotLogic.Create(toAdd);
            }
            return View(model);
        }
    }
}
