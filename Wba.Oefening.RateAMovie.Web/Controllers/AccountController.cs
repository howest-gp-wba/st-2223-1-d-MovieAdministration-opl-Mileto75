using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wba.Oefening.RateAMovie.Web.Data;
using Wba.Oefening.RateAMovie.Core.Entities;
using Wba.Oefening.RateAMovie.Web.ViewModels;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Wba.Oefening.RateAMovie.Web.Services;

namespace Wba.Oefening.RateAMovie.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly MovieContext _movieContext;
        private readonly IAccountService _accountService;
        
        public AccountController(MovieContext movieContext, IAccountService accountService)
        {
            _movieContext = movieContext;
            _accountService = accountService;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AccountRegisterViewModel accountRegisterViewModel)
        {
            //check if username exists
            if(!ModelState.IsValid)
            {
                return View(accountRegisterViewModel);
            }
            
            return RedirectToAction("Registered");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLoginViewModel accountLoginViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(accountLoginViewModel);
            }
            var result = aw
            if (user == null || !Argon2.Verify(user?.Password, accountLoginViewModel.Password))
            {
                ModelState.AddModelError("", "Please provide correct credentials!");
                return View(accountLoginViewModel);
            }
            return RedirectToAction("Index", "Movies");
        }

        [HttpGet]
        public IActionResult Registered()
        {
            return View();
        }
    }
}
