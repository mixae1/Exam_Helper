using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Exam_Helper.ViewsModel.Account
{
    public class UserLogin
    {
        [EmailAddress(ErrorMessage = "Адрес электронной почты некорректен")]
        [Required(ErrorMessage = "Aдрес электронной почты не задан")]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; }
       
        [Required(ErrorMessage = "Пароль не задан")]
        [DataType(DataType.Password, ErrorMessage = "Пароль некорректен")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить")]
        public bool RememberMe { get; set; }
    }
}
