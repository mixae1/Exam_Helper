﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Exam_Helper.ViewsModel.Account;
using Exam_Helper.Models;
using Microsoft.AspNetCore.Authorization;
using Exam_Helper.Services;

namespace Exam_Helper.Controllers
{
    public class UserAccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserAccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public  async Task<IActionResult> Registration(UserRegistration user,string returnUrl="")
        {
            if (ModelState.IsValid)
            {
                User new_user = new User()
                {
                    UserName = user.UserName,
                    Email=user.Login,
                };
                
                var res = await _userManager.CreateAsync(new_user, user.Password);
                if (res.Succeeded)
                {
                    await _signInManager.SignInAsync(new_user, false);
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    return RedirectToAction("Index", "PublicLibrary");


                }

                else
                {  
                    foreach (var error in res.Errors)
                    { 
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View();
        }


        [HttpGet]
        public IActionResult Login(string returnURL = "")
        {
            ViewData["ReturnURL"] = returnURL;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin user,string returnURL="")
        {
            if (ModelState.IsValid)
            {
                var _user = await _userManager.FindByEmailAsync(user.Email);
                if (_user != null)
                {
                    var res = await _signInManager.PasswordSignInAsync(_user, user.Password, false, false);
                    
                    if (res.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(returnURL) && Url.IsLocalUrl(returnURL))
                            return Redirect(returnURL);
                        return RedirectToAction("Index", "PublicLibrary");
                    }
                    else
                    { 
                        ModelState.AddModelError(string.Empty,"incorrect email or password");
                        
                    }

                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // пользователь с данным email может отсутствовать в бд
                    // тем не менее мы выводим стандартное сообщение, чтобы скрыть 
                    // наличие или отсутствие пользователя в бд
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "UserAccount", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(model.Email, "Reset Password",
                    $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>link</a>");
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("ResetPasswordConfirmation");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

    }
}
