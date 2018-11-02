using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class TestingModuleUserManager : UserManager<TestingModuleUser, int>, ITestingModuleUserManager
    {
        public TestingModuleUserManager(ITestingModuleUserStore store) : base(store)
        {
            ClaimsIdentityFactory = new ClaimsIdentityFactory<TestingModuleUser, int>();
            /*DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(10);
            MaxFailedAccessAttemptsBeforeLockout = 10;*/
        }
    }
}