using System;
using System.Collections.Generic;

namespace Wba.Oefening.RateAMovie.Web.ViewModels
{
    public class MoviesInfoViewModel
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public DateTime ReleaseDate { get; set; }
        public IEnumerable<BaseViewModel> Actors { get; set; }
        public IEnumerable<BaseViewModel> Directors { get; set; }
        public BaseViewModel Company { get; set; }
    }
}