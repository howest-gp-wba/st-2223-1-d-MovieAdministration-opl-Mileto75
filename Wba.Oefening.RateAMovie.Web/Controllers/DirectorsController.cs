using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wba.Oefening.RateAMovie.Core.Entities;
using Wba.Oefening.RateAMovie.Web.Data;
using Wba.Oefening.RateAMovie.Web.Services;
using Wba.Oefening.RateAMovie.Web.ViewModels;

namespace Wba.Oefening.RateAMovie.Web.Controllers
{
    public class DirectorsController : Controller
    {
        private readonly MovieContext _movieDbContext;
       

        public DirectorsController(MovieContext movieContext)
        {
            _movieDbContext = movieContext;
        }
        public async Task<IActionResult> Index()
        {
            DirectorsIndexViewModel directorsIndexViewModel =
                new DirectorsIndexViewModel();
            directorsIndexViewModel.Directors = await _movieDbContext
                .Directors.Select(
                d => new BaseViewModel {
                    Id = d.Id,
                    Item = $"{d.FirstName} {d.LastName}"
                }
                ).ToListAsync();
            return View(directorsIndexViewModel);
        }
        public async Task<IActionResult> ShowInfo(long Id)
        {
            PersonShowInfoViewModel directorsShowInfoViewModel
                = new PersonShowInfoViewModel();
            var director = await _movieDbContext.Directors
                .Include(d => d.Movies)
                .FirstOrDefaultAsync(d => d.Id == Id);
            directorsShowInfoViewModel.Id = director.Id;
            directorsShowInfoViewModel.Name = $"{director.FirstName} {director.LastName}";
            directorsShowInfoViewModel.Movies = director.Movies.Select(
                m => new BaseViewModel {Id = m?.Id, Item = m?.Title }
                );
            return View(directorsShowInfoViewModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(DirectorsAddViewModel directorsAddViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(directorsAddViewModel);
            }
            var director = new Director { 
                FirstName = directorsAddViewModel.Firstname,
                LastName = directorsAddViewModel.Lastname
            };
            await _movieDbContext.Directors.AddAsync(director);
            try
            {
                await _movieDbContext.SaveChangesAsync();
                TempData["Message"] = "Director added";
            }
            catch(DbUpdateException exception)
            {
                TempData["Message"] = exception.Message;
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var directorAddViewModel = await _movieDbContext
                .Directors
                .Select(d => new DirectorsAddViewModel { Id = d.Id, Firstname = d.FirstName, Lastname = d.LastName })
                .FirstOrDefaultAsync(d => d.Id == id);
                
            return View(directorAddViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DirectorsAddViewModel directorsAddViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(directorsAddViewModel);
            }
            var director = await _movieDbContext.Directors
                .FirstOrDefaultAsync(d => d.Id == directorsAddViewModel.Id);
            director.FirstName = directorsAddViewModel.Firstname;
            director.LastName = directorsAddViewModel.Lastname;
            await _movieDbContext.SaveChangesAsync();
            TempData["Message"] = "Director edited!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ConfirmDelete(long id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var director = await _movieDbContext
                .Directors
                .FirstOrDefaultAsync(d => d.Id == id);
             _movieDbContext.Directors.Remove(director);
            try
            {
                await _movieDbContext.SaveChangesAsync();
            }
            catch(DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
            }
            TempData["Message"] = "Director deleted!";
            return RedirectToAction("Index");
        }
    }
}
