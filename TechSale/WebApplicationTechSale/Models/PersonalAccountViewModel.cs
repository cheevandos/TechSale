using System.ComponentModel.DataAnnotations;

namespace WebApplicationTechSale.Models
{
    public class PersonalAccountViewModel
    {
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

        [Display(Name = "Имя пользователя в Telegram")]
        public string TelegramId { get; set; }

        public AuctionLotsViewModel PersonalLotsList { get; set; }
    }
}