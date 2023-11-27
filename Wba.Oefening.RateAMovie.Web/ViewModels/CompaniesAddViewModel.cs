using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wba.Oefening.RateAMovie.Web.ViewModels
{
    public class CompaniesAddViewModel
    {
        [Required(ErrorMessage ="Name required")]
        [Display(Name="Company name")]
        public string Name { get; set; }
        [HiddenInput]
        public long? Id { get; set; }
    }
}
