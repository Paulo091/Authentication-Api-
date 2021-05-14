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
        private readonly AppDbContext _dbContext;

        public AccountController( UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        [HttpPost("Register")]
        [Authorize(Roles ="Admin")]
        public async Task<object> Register(RegisterViewModel user)
        {
            var msg = "";

            if (!ModelState.IsValid)
            {
                return BadRequest(new DefaultResponse<string>
                {
                    Success = false,
                    Data = null,
                });
            }

            try
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
            catch(Exception e)
            {
                return new DefaultResponse<string>
                {
                    Success = false,
                    Data = e.Message,
                };
            }
        }

        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<object> Delete(EmailUserViewModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new DefaultResponse<string>
                {
                    Success = false,
                    Data = null,
                });
            }
            try
            {
                var result = _dbContext.Users.Where(x => x.Email == user.Email).FirstOrDefault();

                if (result is null)
                    return NotFound("User not found");

                    await _userManager.DeleteAsync(result);

                return new DefaultResponse<string>
                {
                    Success = true,
                    Data = null,
                };
            }
            catch(Exception e)
            {
                return new DefaultResponse<string>
                {
                    Success = false,
                    Data = e.Message,
                };
            }

        }

        [HttpPut("UpdateUser")]
        [Authorize]
        public async Task<object> Update(RegisterViewModel userToUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new DefaultResponse<string>
                {
                    Success = false,
                    Data = null,
                });
            }
            try
            {
                var userResult = _userManager.Users.Where(x => x.Email == userToUpdate.Email).FirstOrDefault();

                if (userResult is null)
                    return NotFound("Invalid User");

                userResult.Role = ((RoleEnum)userToUpdate.Role).ToString();
                
                var result = await _userManager.UpdateAsync(userResult);

                await _userManager.RemovePasswordAsync(userResult);

                await _userManager.AddPasswordAsync(userResult, userToUpdate.Password);

                return new DefaultResponse<string>
                {
                    Success = true,
                    Data = null,
                };

            }
            catch (Exception e)
            {
                return new DefaultResponse<string>
                {
                    Success = false,
                    Data = e.Message,
                };
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<object> Login(UserViewModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new DefaultResponse<string>
                {
                    Success = false,
                    Data = null,
                });
            }
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
                    var currentUser = _dbContext.Users.Where(x => x.UserName == user.Email).FirstOrDefault();

                    LoginUser.Role = currentUser.Role;

                    var Token = TokenService.GenerateToken(LoginUser);

                    await _signInManager.SignInAsync(LoginUser, isPersistent: false);

                    var response = new DefaultResponse<LoginRequestResponse>
                    {
                        Success = true,
                        Data = new LoginRequestResponse(Token, currentUser.Role),
                    };
                    return response;
                }
                else
                {
                    var response = new DefaultResponse<LoginRequestResponse>
                    {
                        Success = false,
                        Data = new LoginRequestResponse(),                        
                    };
                    return Unauthorized(response);
                }
            }
            catch(Exception e)
            {
                return new DefaultResponse<string>
                {
                    Success = false,
                    Data = e.Message,
                };
            }            
        }

        [HttpGet("ListUsers")]
        [Authorize(Roles ="Admin")]
        public object List()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new DefaultResponse<string>
                {
                    Success = false,
                    Data = null,
                });
            }

            var users = new List<EmailUserViewModel>();

            try
            {
                foreach (var currentUser in _dbContext.Users)
                {
                    users.Add(
                        new EmailUserViewModel
                        {
                            Email = currentUser.Email
                        }
                    );
                }

                return new DefaultResponse<object>
                {
                    Success = true,
                    Data = users
                };

            }catch(Exception e)
            {
                return new DefaultResponse<object>
                {
                    Success = false,
                    Data = e.Message
                };
            }
        }

    }
}
