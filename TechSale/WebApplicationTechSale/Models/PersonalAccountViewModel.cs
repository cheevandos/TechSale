using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationTechSale.Models
{
    public class PersonalAccountViewModel
    {
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

        [Display(Name = "Имя пользователя в Telegram")]
        public string TelegramId { get; set; }

        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}