using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace TestingModule.Additional
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly HttpContext _context;

        public AuthenticationManager()
        {
            _context = HttpContext.Current;
        }

        public Microsoft.Owin.Security.IAuthenticationManager Current => _context.GetOwinContext().Authentication;
    }
}