using System;

namespace DataAccessLogic.DatabaseModels
{
    public class Bid
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int AuctionLotId { get; set; }
        public AuctionLot AuctionLot { get; set; }
    }
}
