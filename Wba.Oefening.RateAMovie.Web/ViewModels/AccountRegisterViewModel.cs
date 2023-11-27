using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wba.Oefening.RateAMovie.Web.ViewModels
{
    public class AccountRegisterViewModel
    {
        [Required(ErrorMessage ="Please provide firstname")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Please provide lastname")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Please provide email")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please provide password")]
        [Display(Name = "Password")]
        [Compare("PasswordCheck",ErrorMessage = "Passwords must match!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please provide repeat password")]
        [Display(Name = "Repeat Password")]
        [DataType(DataType.Password)]
        public string PasswordCheck { get; set; }
    }
}
