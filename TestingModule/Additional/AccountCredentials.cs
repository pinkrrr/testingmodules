using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestingModule.Models;
using TestingModule.ViewModels;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web.Routing;
using TestingModule.Additional;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;

namespace TestingModule.Additional
{
    public class AccountCredentials
    {
        public string GetRole()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value)
                   .SingleOrDefault();
        }

        public async Task<Student> GetStudent()
        {
            testingDbEntities context = new testingDbEntities();
            var claimsIdentity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            int accountId = int.Parse(claimsIdentity.Claims.Where(c => c.Type == "Id").Select(c => c.Value).SingleOrDefault());
            return await context.Students.SingleOrDefaultAsync(s => s.AccountId == accountId);
        }

        public async Task<Lector> GetLector()
        {
            testingDbEntities context = new testingDbEntities();
            var claimsIdentity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            int accountId = int.Parse(claimsIdentity.Claims.Where(c => c.Type == "Id").Select(c => c.Value).SingleOrDefault());
            Lector lector = await context.Lectors.SingleOrDefaultAsync(s => s.AccountId == accountId);
            return lector;
        }
    }
}
