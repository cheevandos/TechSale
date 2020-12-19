using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLogic.CrudLogic
{
    public class AuctionLotPaginationLogic : IPagination<AuctionLot>
    {
        private readonly ApplicationContext context;

        public AuctionLotPaginationLogic(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<int> GetCount()
        {
            return await context.AuctionLots.CountAsync();
        }

        public async Task<List<AuctionLot>> GetPage(int pageNumber)
        {
            return await context.AuctionLots.Include(lot => lot.User).Include(lot => 
            lot.PriceInfo).Skip((pageNumber <= 0 ? 0 : pageNumber - 1) * 
            ApplicationConstantsProvider.GetPageSize())
            .Take(ApplicationConstantsProvider.GetPageSize())
            .ToListAsync();
        }
    }
}
