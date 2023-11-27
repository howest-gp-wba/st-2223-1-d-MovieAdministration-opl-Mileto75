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

namespace Wba.Oefening.RateAMovie.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly MovieContext _movieContext;
        

        public AccountController(MovieContext movieContext)
        {
            _movieContext = movieContext;
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
            if( _movieContext.Users.Any(u => u.Username.Equals(accountRegisterViewModel.Username)))
            {
                ModelState.AddModelError("", "Credentials seem to exist in database. Would you like to request a password reset?");
            }
            if(!ModelState.IsValid)
            {
                return View(accountRegisterViewModel);
            }
            User user = new User();
            user.Username = accountRegisterViewModel.Username;
            user.FirstName = accountRegisterViewModel.Firstname;
            user.LastName = accountRegisterViewModel.Lastname;
            user.Password = Argon2.Hash(accountRegisterViewModel.Password);
            //add to the context
            _movieContext.Users.Add(user);
            try
            {
                await _movieContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                //perform logging here in production environment
                Console.WriteLine(e.Message);
                ModelState.AddModelError("", "Something went wrong...Please try again later..");
                return View();
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
            //check user  and
            //check credentials
            var user = await _movieContext.Users.FirstOrDefaultAsync(u => u.Username.Equals(accountLoginViewModel.Username));

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
