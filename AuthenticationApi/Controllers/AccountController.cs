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
        [HttpPost("Register")]
        public async Task<object> Register()
        {
            return "";
        }

        [HttpPost("Login")]
        public async Task<object> Login()
        {
            return "";
        }

    }
}
