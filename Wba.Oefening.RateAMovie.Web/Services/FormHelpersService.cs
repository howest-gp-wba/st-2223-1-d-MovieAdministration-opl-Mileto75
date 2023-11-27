using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wba.Oefening.RateAMovie.Core.Entities;
using Wba.Oefening.RateAMovie.Web.Data;
using Wba.Oefening.RateAMovie.Web.Models;

namespace Wba.Oefening.RateAMovie.Web.Services
{
    public class FormHelpersService
    {
        private readonly MovieContext _movieContext;

        public FormHelpersService(MovieContext movieContext)
        {
            _movieContext = movieContext;
        }

        public async Task<List<SelectListItem>> BuildCompanyList()
        {
            return await _movieContext
                .Companies
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<CheckboxHelper>> BuildCheckboxList(bool isActor)
        {
            if (isActor)
            {
                return await _movieContext.Actors.Select(a => new CheckboxHelper
                {
                    Id = a.Id,
                    Text = $"{a.FirstName} {a.LastName}"
                }).ToListAsync();
            }
            return await _movieContext.Directors.Select(d => new CheckboxHelper
                {
                    Id = d.Id,
                    Text = $"{d.FirstName} {d.LastName}"
                }).ToListAsync();
        }

        public async Task<List<CheckboxHelper>> BuildCheckboxList(bool isActor,Movie editMovie)
        {
            var checkboxList = await BuildCheckboxList(isActor);
            foreach(var checkbox in checkboxList)
            {
                if(isActor)
                {
                    if (editMovie.Actors.Any(m => m.Id == checkbox.Id))
                    {
                        checkbox.IsSelected = true;
                    }
                }
                else
                {
                    if (editMovie.Directors.Any(m => m.Id == checkbox.Id))
                    {
                        checkbox.IsSelected = true;
                    }
                }
            }
            return checkboxList;
        }
    }
}
