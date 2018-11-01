using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestingModule.Additional
{
    public interface IAuthenticationManager : IDependency
    {
        Microsoft.Owin.Security.IAuthenticationManager Current { get; }
    }
}