using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wba.Oefening.RateAMovie.Web.ViewModels
{
    public class ActorsAddActorViewModel
    {
        [Required(ErrorMessage = "Firstname required!")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Lastname required!")]
        public string Lastname { get; set; }
        public long? Id { get; set; }
    }
}
