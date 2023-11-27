using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wba.Oefening.RateAMovie.Web.ViewModels
{
    public class MoviesIndexViewModel
    {
        public IEnumerable<BaseViewModel> Movies { get; set; }
    }
}
