using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wba.Oefening.RateAMovie.Web.ViewModels
{
    public class DirectorsAddViewModel
    {
        [Required(ErrorMessage ="Firstname required!")]
        [MaxLength(200)]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Lastname required!")]
        [MaxLength(200)]
        public string Lastname { get; set; }
        [HiddenInput]
        public long Id { get; set; }
    }
}
