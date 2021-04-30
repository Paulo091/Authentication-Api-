using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApi.Models
{
    public class DefaultResponse <T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
    }
}
