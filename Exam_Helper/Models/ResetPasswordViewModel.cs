using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Exam_Helper.Models
{
    public class ResetPasswordViewModel
    {
        /*
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Пароль должен содержать как минимум 6 символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        */
        [Required(ErrorMessage = "Пароль не задан")]
        [DataType(DataType.Password, ErrorMessage = "Пароль некорректен")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтверждение пароль не задано")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password, ErrorMessage = "Пароль некорректен")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}