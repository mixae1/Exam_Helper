using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Exam_Helper.ViewsModel.Account
{
    public class UserLogin
    {
        [EmailAddress]
        [Required]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; }
       
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить")]
        public bool RememberMe { get; set; }
    }
}
