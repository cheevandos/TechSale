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
    public class AuctionLotLogic : ICrudLogic<AuctionLot>
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

            toUpdate.Name = model.Name;
            toUpdate.Description = model.Description;
            toUpdate.StartDate = model.StartDate;
            toUpdate.EndDate = model.EndDate;
            toUpdate.PhotoSrc = model.PhotoSrc;

            await context.SaveChangesAsync();
        }

        public List<AuctionLot> Read(AuctionLot model)
        {
            return context.AuctionLots.Where(lot => model == null
            || lot.User.UserName == model.User.UserName
            || lot.Id == model.Id
            || lot.Status == LotStatusProvider.GetOnModerationStatus())
                .ToList();
        }
    }
}
