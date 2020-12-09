using System.Collections.Generic;

namespace DataAccessLogic.DatabaseModels
{
    public class SavedList
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<AuctionLot> AuctionLots { get; set; }
    }
}
