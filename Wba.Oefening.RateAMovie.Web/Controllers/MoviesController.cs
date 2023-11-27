using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wba.Oefening.RateAMovie.Core.Entities;
using Wba.Oefening.RateAMovie.Web.Data;
using Wba.Oefening.RateAMovie.Web.Models;
using Wba.Oefening.RateAMovie.Web.Services;
using Wba.Oefening.RateAMovie.Web.ViewModels;

namespace Wba.Oefening.RateAMovie.Web.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MovieContext _movieContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;
        private readonly IFormHelpersService _formHelpersService;

        public MoviesController(MovieContext movieContext,
            IWebHostEnvironment webHostEnvironment,
            IFormHelpersService formHelpersService,
            IFileService fileService)
        {
            //injection here!!
            _movieContext = movieContext;
            _webHostEnvironment = webHostEnvironment;
            _formHelpersService = formHelpersService;
            _fileService = fileService;
        }
        public async Task<IActionResult> Index()
        {
            MoviesIndexViewModel moviesIndexViewModel = new();
            moviesIndexViewModel.Movies = await _movieContext
                .Movies
                .Select(m => new BaseViewModel
                {
                    Id = m.Id,
                    Item = m.Title
                }).ToListAsync();
            return View(moviesIndexViewModel);
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
            //delete movie
            var movie = await _movieContext
                .Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            _movieContext.Movies.Remove(movie);
            await _movieContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Info(long id)
        {
            //get the movie and put in model
            MoviesInfoViewModel moviesInfoViewModel = new();
            var movie = await _movieContext.Movies
                .Include(m => m.Company)
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .FirstOrDefaultAsync(m => m.Id == id);
            moviesInfoViewModel.Id = movie?.Id ?? 0;
            moviesInfoViewModel.Title = movie?.Title ?? "<noTitle>";
            moviesInfoViewModel.ReleaseDate = movie?.ReleaseDate ?? DateTime.Now;
            moviesInfoViewModel.ImagePath = movie?.ImageFileName ?? $"{_webHostEnvironment.WebRootPath}/images/placeholder.jpg";
            moviesInfoViewModel.Company = new BaseViewModel 
            {
                Id = movie?.CompanyId ?? 0,
                Item = movie?.Company?.Name ?? "<NoName>" 
            };
            moviesInfoViewModel.Actors = movie?.Actors.Select(
                a => new BaseViewModel 
                {
                    Id = a?.Id,
                    Item = $"{a?.FirstName} {a?.LastName}"
                }
                );
            moviesInfoViewModel.Directors = movie?.Directors.Select(
                a => new BaseViewModel
                {
                    Id = a?.Id,
                    Item = $"{a?.FirstName} {a?.LastName}"
                }
                );
            ViewBag.PageTitle = movie?.Title ?? "<noTitle>";
            return View(moviesInfoViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            MoviesAddUpdateMovieViewModel moviesAddMovieViewModel = new();
            moviesAddMovieViewModel.Actors =
                await _formHelpersService.BuildCheckboxList(true);
            moviesAddMovieViewModel.Directors =
                await _formHelpersService.BuildCheckboxList(false);
            moviesAddMovieViewModel.Companies
                = await _formHelpersService.BuildCompanyList();
            moviesAddMovieViewModel.ReleaseDate = DateTime.Now.Date;
            return View(moviesAddMovieViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(MoviesAddUpdateMovieViewModel moviesAddMovieViewModel)
        {

            //check custom errors
            if (moviesAddMovieViewModel.ReleaseDate > DateTime.Now)
            {
                ModelState.AddModelError("", "Releasedate must be in the past!");
            }
            if (!ModelState.IsValid)
            {
                moviesAddMovieViewModel.Companies
                = await _formHelpersService.BuildCompanyList();
                return View(moviesAddMovieViewModel);
            }
            //store movie here
            Movie newMovie = new();
            newMovie.Title = moviesAddMovieViewModel.Title;
            newMovie.CompanyId = moviesAddMovieViewModel.CompanyId;
            newMovie.ReleaseDate = moviesAddMovieViewModel.ReleaseDate;
            if (moviesAddMovieViewModel.Image != null)
            {
                //check file extension
                if(!Path.GetExtension(moviesAddMovieViewModel.Image.FileName).Equals(".jpg"))
                {
                    moviesAddMovieViewModel.Companies
                = await _formHelpersService.BuildCompanyList();
                    ModelState.AddModelError("", "Only jpg allowed!");
                    return View(moviesAddMovieViewModel);
                }
                //call StoreFile service method
                newMovie.ImageFileName = await _fileService.AddOrUpdateFile(moviesAddMovieViewModel.Image,
                    "movies", _webHostEnvironment);
            }
            //store actors
            var selectedActors = moviesAddMovieViewModel
                .Actors
                .Where(a => a.IsSelected == true).Select(a => a.Id);
            newMovie.Actors = await _movieContext
                .Actors
                .Where(a => selectedActors.Contains(a.Id)).ToListAsync();
            
            //store directors
            var selectedDirectors = moviesAddMovieViewModel
                .Directors.Where(d => d.IsSelected == true)
                .Select(d => d.Id);
            newMovie.Directors = await _movieContext
                .Directors
                .Where(d => selectedDirectors.Contains(d.Id)).ToListAsync();
            _movieContext.Movies.Add(newMovie);
            await _movieContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            //reuse the AddMovieViewModel
            MoviesAddUpdateMovieViewModel moviesUpdateMovieViewModel
                = new();
            //get the movie
            var movie = await _movieContext
                .Movies
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .FirstOrDefaultAsync(m => m.Id == id);
            //fill the model
            moviesUpdateMovieViewModel.Title = movie?.Title ?? "<NoTitle>";
            moviesUpdateMovieViewModel.ReleaseDate = movie?.ReleaseDate ?? DateTime.Today;
            moviesUpdateMovieViewModel.Id = movie?.Id ?? 0;
            moviesUpdateMovieViewModel.CompanyId = movie?.CompanyId ?? 0;
            //build the checkboxes
            moviesUpdateMovieViewModel.Actors
                = await _formHelpersService.BuildCheckboxList(true, movie);
            
            //directors
            moviesUpdateMovieViewModel.Directors
                = await _formHelpersService.BuildCheckboxList(false, movie);

            //build the companylist
            moviesUpdateMovieViewModel.Companies = await _formHelpersService
                .BuildCompanyList();
            return View(moviesUpdateMovieViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MoviesAddUpdateMovieViewModel moviesUpdateMovieViewModel)
        {
            //do some custom checks
            if (moviesUpdateMovieViewModel.ReleaseDate > DateTime.Today)
            {
                ModelState.AddModelError("", "Dat must be in the past!");
            }
            if(moviesUpdateMovieViewModel.Image != null &&
                !Path.GetExtension(moviesUpdateMovieViewModel.Image?.FileName).Equals(".jpg"))
            {
                ModelState.AddModelError("", "image must be .jpg!");
            }
            if (!ModelState.IsValid)
            {
                //repopulate the company selectlist
                moviesUpdateMovieViewModel.Companies = await _formHelpersService
                    .BuildCompanyList();
                return View(moviesUpdateMovieViewModel);
            }
           
            //update movie
            var movie = await _movieContext
                .Movies
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .FirstOrDefaultAsync(m => m.Id == moviesUpdateMovieViewModel.Id);
            //update everything
            movie.Title = moviesUpdateMovieViewModel.Title;
            movie.ReleaseDate = moviesUpdateMovieViewModel.ReleaseDate;
            movie.CompanyId = moviesUpdateMovieViewModel.CompanyId;
            //actors
            var selectedActors = moviesUpdateMovieViewModel
                .Actors
                .Where(a => a.IsSelected == true)
                .Select(a => a.Id);
            movie.Actors =
                await _movieContext
                .Actors.Where(a => selectedActors.Contains(a.Id)).ToListAsync();

            //directors
            var selectedDirectors = moviesUpdateMovieViewModel.Directors
                .Where(d => d.IsSelected == true).Select(d => d.Id);
            movie.Directors = await _movieContext
                .Directors.Where(d => selectedDirectors.Contains(d.Id)).ToListAsync();

            //update image if not null
            if(moviesUpdateMovieViewModel.Image != null)
            {
                //check if existing movie had an image in the first place
                if (movie.ImageFileName == null)
                {
                    movie.ImageFileName = await _fileService.AddOrUpdateFile(moviesUpdateMovieViewModel.Image,
                        "movies",_webHostEnvironment);
                }
                else
                {
                    movie.ImageFileName = await _fileService
                        .AddOrUpdateFile(moviesUpdateMovieViewModel.Image, "movies",_webHostEnvironment, movie.ImageFileName);
                }
            }
            //save
            await _movieContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}