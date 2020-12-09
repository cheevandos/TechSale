using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WebApplicationTechSale.Models
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Укажите адрес электронной почты")]
        [Display(Name = "Адрес электронной почты")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Неверный формат")]

        public string Email { get; set; }
        [Required(ErrorMessage = "Придумайте пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirmation { get; set; }
    }
}
