using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wba.Oefening.RateAMovie.Web.Models
{
    public class CheckboxHelper
    {
        public bool  IsSelected { get; set; }
        [HiddenInput]
        public long Id { get; set; }
        [HiddenInput]
        public string Text { get; set; }
    }
}
