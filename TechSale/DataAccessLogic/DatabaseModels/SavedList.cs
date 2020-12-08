using System.Collections.Generic;

namespace WebApplicationTechSale.Models
{
    public class SavedList
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<AuctionLot> AuctionLots { get; set; }
    }
}
