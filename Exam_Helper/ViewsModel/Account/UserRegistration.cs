using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel.Account
{
    public class UserRegistration
    {
        [Required(ErrorMessage ="Логин или aдрес электронной почты не задан.")]
        [Display(Name = "Логин или электронная почта")]
        [EmailAddress]
        public string Login { get; set; }

        [Required(ErrorMessage ="Имя пользователя не задано")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        public string PasswordConfirm { get; set; }
    }
}
