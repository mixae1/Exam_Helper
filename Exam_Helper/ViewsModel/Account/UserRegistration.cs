using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Exam_Helper.ViewsModel.Account
{
    public class UserRegistration
    {
        [Required(ErrorMessage ="Адрес электронной почты не задан")]
        [Display(Name = "Электронная почта")]
        [EmailAddress(ErrorMessage = "Адрес электронной почты некорректен")]
        public string Login { get; set; }

        [Required(ErrorMessage ="Имя пользователя не задано")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Пароль не задан")]
        [DataType(DataType.Password, ErrorMessage = "Пароль некорректен")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Нет подтверждения пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password, ErrorMessage = "Пароль некорректен")]
        [Display(Name = "Подтверждение пароля")]
        public string PasswordConfirm { get; set; }
    }
}
