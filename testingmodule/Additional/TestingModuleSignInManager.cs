using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TestingModule.Models;


namespace TestingModule.Additional
{
    public class TestingModuleSignInManager : SignInManager<TestingModuleUser, int>
    {
        public TestingModuleSignInManager(TestingModuleUserManager userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(TestingModuleUser user)
        {
            return user.GenerateUserIdentityAsync(UserManager);
        }
    }
}