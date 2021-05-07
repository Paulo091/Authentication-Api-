using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApi.Models.RequestResponses
{
    public class LoginRequestResponse
    {
        public string JwtToken { get; private set; }
        public string Role { get; private set; }

        public LoginRequestResponse(string JwtToken, string Role)
        {
            this.JwtToken = JwtToken;
            this.Role = Role;
        }
    }
}
