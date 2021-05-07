using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; }
    }
}
