using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLogic.CrudLogic
{
    public class AuctionLotLogic : ICrudLogic<AuctionLot>, IPagination<AuctionLot>
    {
        private readonly ApplicationContext context;

        public AuctionLotLogic(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task Create(AuctionLot model)
        {
            if (model.User == null || string.IsNullOrWhiteSpace(model.User.UserName))
            {
                throw new Exception("Пользователь не определен");
            }

            AuctionLot sameLot = await context.AuctionLots
                .Include(lot => lot.User)
                .FirstOrDefaultAsync(lot =>
                lot.User.UserName == model.User.UserName && lot.Name == model.Name);
            if (sameLot != null)
            {
                throw new Exception("Уже есть лот с таким названием");
            }

            model.Status = LotStatusProvider.GetOnModerationStatus();
            model.Id = Guid.NewGuid().ToString();
            model.User = await context.Users.FirstAsync(user => 
            user.UserName == model.User.UserName);

            await context.AuctionLots.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task Delete(AuctionLot model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                throw new Exception("Лот не определен");
            }

            AuctionLot toDelete = await context.AuctionLots.FirstOrDefaultAsync(lot =>
            lot.Id == model.Id);
            if (toDelete == null)
            {
                throw new Exception("Лот не найден");
            }

            context.AuctionLots.Remove(toDelete);
            await context.SaveChangesAsync();
        }

        public async Task Update(AuctionLot model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                throw new Exception("Лот не определен");
            }

            AuctionLot toUpdate = await context.AuctionLots.FirstOrDefaultAsync(lot =>
            lot.Id == model.Id); 
            if (toUpdate == null)
            {
                throw new Exception("Лот не найден");
            }
            
            if (!string.IsNullOrWhiteSpace(model.Status))
            {
                toUpdate.Status = model.Status;
            }
            else
            {
                toUpdate.Name = model.Name;
                toUpdate.Description = model.Description;
                toUpdate.StartDate = model.StartDate;
                toUpdate.EndDate = model.EndDate;
                toUpdate.PhotoSrc = model.PhotoSrc;
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<AuctionLot>> Read(AuctionLot model)
        {
            return await context.AuctionLots.Include(lot => lot.User).Include(lot => lot.Bids).Where(lot => model == null
            || model.User != null && !string.IsNullOrWhiteSpace(model.User.UserName) && lot.User.UserName == model.User.UserName
            || !string.IsNullOrWhiteSpace(model.Id) && lot.Id == model.Id
            || !string.IsNullOrWhiteSpace(model.Status) && lot.Status == model.Status)
            .ToListAsync();
        }

        public async Task<List<AuctionLot>> GetPage(int pageNumber, AuctionLot model)
        {
            return await context.AuctionLots.Include(lot => lot.User).Include(lot =>
            lot.PriceInfo).Where(lot => model == null
            || !string.IsNullOrWhiteSpace(model.Status) && lot.Status == model.Status
            || model.User != null && lot.User == model.User)
            .OrderByDescending(lot => lot.StartDate)
            .Skip((pageNumber <= 0 ? 0 : pageNumber - 1) *
            ApplicationConstantsProvider.GetPageSize())
            .Take(ApplicationConstantsProvider.GetPageSize())
            .ToListAsync();
        }

        public async Task<int> GetCount(AuctionLot model)
        {
            return await context.AuctionLots.CountAsync(lot => model == null 
            || !string.IsNullOrWhiteSpace(model.Status) && lot.Status == model.Status
            || model.User != null && lot.User == model.User);
        }
    }
}
