using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public interface ITestingDbEntityService : IDependency
    {
        testingDbEntities GetContext();
    }

    public class TestingDbEntityService : ITestingDbEntityService
    {
        public testingDbEntities GetContext()
        {
            return new testingDbEntities();
        }
    }
}