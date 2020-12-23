using System.ComponentModel.DataAnnotations;

namespace WebApplicationTechSale.Models
{
    public class UpdateAccountViewModel
    {
        [Required(ErrorMessage = "Введите адрес электронной почты")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Неверный формат")]
        [Display(Name = "Новый адрес электронной почты")]
        public string NewEmail { get; set; }

        [Display(Name = "Имя пользователя в Telegram")]
        public string NewTelegramUserName { get; set; }
    }
}
