using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wba.Oefening.RateAMovie.Web.ViewModels
{
    public class PersonShowInfoViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<BaseViewModel> Movies { get; set; }
    }
}
