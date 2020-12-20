using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    [Authorize(Roles = "moderator")]
    public class ModeratorController : Controller
    {
        private readonly IPagination<AuctionLot> paginationLotLogic;
        private readonly ICrudLogic<AuctionLot> crudLotLogic;
        private readonly ICrudLogic<Note> crudNoteLogic;


        public ModeratorController(IPagination<AuctionLot> paginationLotLogic, 
            ICrudLogic<AuctionLot> crudLotLogic, ICrudLogic<Note> crudNoteLogic)
        {
            this.paginationLotLogic = paginationLotLogic;
            this.crudLotLogic = crudLotLogic;
            this.crudNoteLogic = crudNoteLogic;
        }

        [HttpGet]
        public async Task<IActionResult> Lots(int page = 1)
        {
            List<AuctionLot> lotsOnModeration = await paginationLotLogic.GetPage(page, new AuctionLot
            {
                Status = LotStatusProvider.GetOnModerationStatus()
            });

            int lotsCount = await paginationLotLogic.GetCount();

            return View(new AuctionLotsViewModel
            {
                AuctionLots = lotsOnModeration,
                PageViewModel = new PageViewModel(lotsCount, page, ApplicationConstantsProvider.GetPageSize())
            });
        }

        [HttpGet]
        public IActionResult CheckLot(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                AuctionLot lotToCheck = crudLotLogic.Read(new AuctionLot
                {
                    Id = id
                })?.First();
                return View(lotToCheck);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AcceptLot(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                await crudLotLogic.Update(new AuctionLot
                {
                    Id = id,
                    Status = LotStatusProvider.GetAcceptedStatus()
                });
                return RedirectToAction("Lots", "Moderator");
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> RejectLot(RejectLotModel model)
        {
            if (ModelState.IsValid)
            {
                await crudLotLogic.Update(new AuctionLot
                {
                    Id = model.Id,
                    Status = LotStatusProvider.GetRejectedStatus()
                });
                await crudNoteLogic.Create(new Note
                {
                    AuctionLotId = model.Id,
                    Text = model.Note
                });
                return RedirectToAction("Lots", "Moderator");
            }
            return NotFound();
        }
    }
}
