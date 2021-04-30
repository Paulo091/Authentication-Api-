using AuthenticationApi.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController( UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<object> Register(RegisterViewModel user)
        {
            var msg = "";
            if (ModelState.IsValid)
            {
                var identityUser = new IdentityUser() 
                { 
                    UserName = user.Email,
                    Email = user.Email                   
                };

                var createUserResult = await _userManager.CreateAsync(identityUser, user.Password);

                if (createUserResult.Succeeded)
                    return "Success";
                else
                {
                    msg = "Error List:";
                    foreach (var error in createUserResult.Errors)
                    {
                        msg += $"\n {error.Description}";
                    }

                    return BadRequest(msg);
                }

            }
            return BadRequest("");
        }

        [HttpPost("Login")]
        public async Task<object> Login()
        {
            return "";
        }

    }
}
