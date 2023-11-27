using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class ActorsController : Controller
    {
        private readonly MovieContext _movieDbContext;
        
        public ActorsController(MovieContext movieContext)
        {
            _movieDbContext = movieContext;
        }
        public async Task<IActionResult> Index()
        {
            ActorsIndexViewModel actorsIndexViewModel
                = new ActorsIndexViewModel();
            actorsIndexViewModel.Actors = await _movieDbContext
                .Actors.Select(a => new BaseViewModel
                {
                    Id = a.Id,
                    Item = $"{a.FirstName} {a.LastName}"
                }).ToListAsync();
            return View(actorsIndexViewModel);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(ActorsAddActorViewModel actorsAddActorViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(actorsAddActorViewModel);
            }
            //save new actor
            var newActor = new Actor
            {
                FirstName = actorsAddActorViewModel?.Firstname,
                LastName = actorsAddActorViewModel?.Lastname
            };
            //add to context
            _movieDbContext.Actors.Add(newActor);
            //save
            try
            {
                _movieDbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
            }
            TempData["Message"] = "Actor added";
            return RedirectToAction("Index", "Actors");
        }

        [HttpGet]
        public IActionResult ConfirmDelete(long Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        [HttpGet]
        public IActionResult Delete(long Id)
        {
            var deleteActor = _movieDbContext
                .Actors.FirstOrDefault(a => a.Id == Id);
            _movieDbContext.Actors.Remove(deleteActor);
            try
            {
                _movieDbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
            }
            TempData["Message"] = "Actor deleted";
            return RedirectToAction("Index", "Actors");
        }

        [HttpGet]
        public IActionResult Edit(long Id)
        {
            ActorsAddActorViewModel actorsAddActorViewModel = new ActorsAddActorViewModel();
            actorsAddActorViewModel.Id = Id;
            var updateActor = _movieDbContext.Actors.FirstOrDefault(a => a.Id == Id);
            actorsAddActorViewModel.Firstname = updateActor?.FirstName;
            actorsAddActorViewModel.Lastname = updateActor?.LastName;
            return View(actorsAddActorViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ActorsAddActorViewModel actorsAddActorViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(actorsAddActorViewModel);
            }
            //update Actor
            var updateActor = _movieDbContext.Actors
                .FirstOrDefault(a => a.Id == actorsAddActorViewModel.Id);
            updateActor.FirstName = actorsAddActorViewModel?.Firstname;
            updateActor.LastName = actorsAddActorViewModel?.Lastname;
            //savechanges
            try
            {
                _movieDbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
            }
            TempData["Message"] = "Actor edited";
            return RedirectToAction("Index", "Actors");
        }
        public async Task<IActionResult> ShowInfo(long Id)
        {
            PersonShowInfoViewModel directorsShowInfoViewModel
                = new PersonShowInfoViewModel();
            var director = await _movieDbContext.Actors
                .Include(d => d.Movies)
                .FirstOrDefaultAsync(d => d.Id == Id);
            directorsShowInfoViewModel.Id = director.Id;
            directorsShowInfoViewModel.Name = $"{director.FirstName} {director.LastName}";
            directorsShowInfoViewModel.Movies = director.Movies.Select(
                m => new BaseViewModel { Id = m?.Id, Item = m?.Title }
                );
            return View(directorsShowInfoViewModel);
        }
    }

}
