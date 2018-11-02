using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public interface ITestingModuleUserManager : IDependency
    {
        Task<TestingModuleUser> FindByNameAsync(string userName);
    }
}