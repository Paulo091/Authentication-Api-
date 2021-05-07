using AuthenticationApi.DbContext;
using AuthenticationApi.Models;
using AuthenticationApi.Models.Enums;
using AuthenticationApi.Models.RequestResponses;
using AuthenticationApi.Services;
using AuthenticationApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController( UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        [Authorize(Roles ="Admin")]
        public async Task<object> Register(RegisterViewModel user)
        {
            var msg = "";
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser() 
                { 
                    UserName = user.Email,
                    Email = user.Email,
                    Role = ((RoleEnum)user.Role).ToString()
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
        [AllowAnonymous]
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
                    var currentUser = _signInManager.UserManager.Users.Where(x => x.UserName == user.Email).FirstOrDefault();

                    LoginUser.Role = currentUser.Role;

                    var Token = TokenService.GenerateToken(LoginUser);

                    await _signInManager.SignInAsync(LoginUser, isPersistent: false);



                    LoginRequestResponse loginRequestResponse = new LoginRequestResponse(Token, currentUser.Role);

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
