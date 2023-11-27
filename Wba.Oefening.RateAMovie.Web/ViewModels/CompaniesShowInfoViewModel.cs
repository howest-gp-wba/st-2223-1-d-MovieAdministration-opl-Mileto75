using System.Collections.Generic;

namespace Wba.Oefening.RateAMovie.Web.ViewModels
{
    public class CompaniesShowInfoViewModel
    {
        public string Name { get; set; }
        public IEnumerable<BaseViewModel> Movies { get; set; }
    }
}
