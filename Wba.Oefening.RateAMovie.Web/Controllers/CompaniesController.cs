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
    public class CompaniesController : Controller
    {
        private readonly MovieContext _movieDbContext;
        

        public CompaniesController(MovieContext movieContext)
        {
            _movieDbContext = movieContext;
        }
        public async Task<IActionResult> Index()
        {
            CompaniesIndexViewModel companiesIndexViewModels = new CompaniesIndexViewModel();
                companiesIndexViewModels.Companies
                = await _movieDbContext
                .Companies
                .Select(c => new BaseViewModel
                {
                    Item = c.Name,
                    Id = c.Id
                }).ToListAsync();
            
            return View(companiesIndexViewModels);
        }
        
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CompaniesAddViewModel companiesAddViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(companiesAddViewModel);
            }
            var company = new Company
            {
                Name = companiesAddViewModel.Name
            };
            await _movieDbContext.Companies.AddAsync(company);
            try
            {
                await _movieDbContext.SaveChangesAsync();
                TempData["Message"] = "Company added";
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
            }
            TempData["Message"] = "Company added";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            CompaniesAddViewModel companiesAddViewModel = new();
            var company = await _movieDbContext
                .Companies
                .FirstOrDefaultAsync(c => c.Id == id);
            if(company == null)
            {
                TempData["Message"] = "something went wrong...";
                return RedirectToAction("Index");
            }
            companiesAddViewModel.Name = company.Name;
            companiesAddViewModel.Id = company.Id;
            return View(companiesAddViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompaniesAddViewModel companiesUpdateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(companiesUpdateViewModel);
            }
            var company = await _movieDbContext.Companies
                .FirstOrDefaultAsync(c => c.Id == companiesUpdateViewModel.Id);
            company.Name = companiesUpdateViewModel.Name;
            await _movieDbContext.SaveChangesAsync();
            TempData["Message"] = "Company edited!";
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
            var company = await _movieDbContext
                .Companies
                .FirstOrDefaultAsync(c => c.Id == id);
            _movieDbContext.Companies.Remove(company);
            try
            {
                await _movieDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
            }
            TempData["Message"] = "Company deleted!";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ShowInfo(long Id)
        {
            CompaniesShowInfoViewModel companiesShowInfoViewModel
                = new CompaniesShowInfoViewModel();
            var company = await _movieDbContext.Companies
                .Include(d => d.Movies)
                .FirstOrDefaultAsync(d => d.Id == Id);
            companiesShowInfoViewModel.Name = company.Name;
            companiesShowInfoViewModel.Movies = company.Movies.Select(
                m => new BaseViewModel { Id = m?.Id, Item = m?.Title }
                );
            return View(companiesShowInfoViewModel);
        }
    }
}
