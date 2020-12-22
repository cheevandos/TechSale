using DataAccessLogic.DatabaseModels;
using DataAccessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLogic.CrudLogic
{
    public class BidLogic : ICrudLogic<Bid>
    {
        private readonly ApplicationContext context;

        public BidLogic(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task Create(Bid model)
        {
            if (string.IsNullOrWhiteSpace(model.AuctionLotId))
            {
                throw new Exception("Лот не определен");
            }
            
        }

        public Task Delete(Bid model)
        {
            throw new NotImplementedException();
        }

        public Task Update(Bid model)
        {
            throw new NotImplementedException();
        }

        public Task<List<Bid>> Read(Bid model)
        {
            throw new NotImplementedException();
        }
    }
}
