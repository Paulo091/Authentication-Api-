using AuthenticationApi.Models;
using AuthenticationApi.Models.RequestResponses;
using AuthenticationApi.Services;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController( UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<object> Register(RegisterViewModel user)
        {
            var msg = "";
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser() 
                { 
                    UserName = user.Email,
                    Email = user.Email,
                    Role = user.Role
                };

                var createUserResult = await _userManager.CreateAsync(applicationUser, user.Password);

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
        public async Task<object> Login(LoginViewModel user)
        {
            try
            {
                var LoginUser = new ApplicationUser
                {
                    UserName = user.Email,
                    Email = user.Email
                };
                var result = await _signInManager.PasswordSignInAsync(user.Email,user.Password,true,false);

                if (result.Succeeded)
                {

                    var Token = TokenService.GenerateToken(LoginUser);

                    LoginRequestResponse loginRequestResponse = new LoginRequestResponse(Token);

                    var response = new DefaultResponse<LoginRequestResponse>
                    {
                        Success = true,
                        Data = loginRequestResponse,
                    };
                    return response;
                }
                else
                {
                    var response = new DefaultResponse<string>
                    {
                        Success = false,
                        Data = "Invalid Credentials",                        
                    };
                    return Unauthorized(response);
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }            
        }

    }
}
