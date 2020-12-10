using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLogic.DatabaseModels
{
    public class AuctionLot
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Введите название")]
        [MaxLength(100, ErrorMessage = "Слишком много символов")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Добавьте описание")]
        [MaxLength(1000, ErrorMessage = "Слишком много символов")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Укажите дату начала")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Укажите дату окончания")]
        public DateTime EndDate { get; set; }
        public string PhotoSrc { get; set; }
        public string Status { get; set; }

        public PriceInfo PriceInfo { get; set; }

        public Note Note { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public List<SavedList> SavedLists { get; set; }

        public List<Bid> Bids { get; set; }
    }
}
