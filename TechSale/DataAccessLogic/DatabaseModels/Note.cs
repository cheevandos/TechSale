using System.ComponentModel.DataAnnotations;

namespace WebApplicationTechSale.Models
{
    public class Note
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите текст сообщения")]
        [MaxLength(250, ErrorMessage = "Слишком длинное сообщение")]
        public string Text { get; set; }

        public int AuctionLotId { get; set; }
        public AuctionLot AuctionLot { get; set; }
    }
}
