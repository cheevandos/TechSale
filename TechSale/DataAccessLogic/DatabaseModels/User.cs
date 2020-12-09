using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DataAccessLogic.DatabaseModels
{
    public class User : IdentityUser
    {
        public string TelegramId { get; set; }
        public SavedList SavedList { get; set; }
        public List<Bid> Bids { get; set; }
        public List<AuctionLot> AuctionLots { get; set; }
    }
}
