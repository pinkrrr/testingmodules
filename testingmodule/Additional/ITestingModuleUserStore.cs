using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public interface ITestingModuleUserStore : IDependency, IUserStore<TestingModuleUser, int>
    {
    }
}