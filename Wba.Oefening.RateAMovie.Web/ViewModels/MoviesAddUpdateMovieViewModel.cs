using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Wba.Oefening.RateAMovie.Web.Models;

namespace Wba.Oefening.RateAMovie.Web.ViewModels
{
    public class MoviesAddUpdateMovieViewModel
    {
        [HiddenInput]
        public long? Id { get; set; }
        [Required(ErrorMessage = "Title required!")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Date required!")]
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }
        [DataType(DataType.Upload)]
        
        public IFormFile Image { get; set; }
        public List<CheckboxHelper> Actors { get; set; }
        public List<CheckboxHelper> Directors { get; set; }
        public List<SelectListItem> Companies { get; set; }
        public long? CompanyId { get; set; }
    }
}
