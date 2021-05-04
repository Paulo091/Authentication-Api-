using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApi.ViewModels
{
    public class RegisterViewModel
    {
        public string Email { get; set; }
        public int Role { get; set; }
        public string Password { get; set; }
        [Compare("Password", ErrorMessage ="Password and Confirmation Password not match")]
        public string ConfirmPassword { get; set; }
    }
}
