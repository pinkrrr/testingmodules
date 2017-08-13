using System;
using System.Collections.Generic;
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

namespace TestingModule.Additional
{
    public class AccountCredentials
    {
        public string GetRole()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();
           // return claimsIdentity.FindFirst(ClaimTypes.Role).Value;
         }

        public async Task<Student> GetStudent()
        {
            testingDbEntities _context=new testingDbEntities();
            var claimsIdentity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var claimList = claimsIdentity.Claims.Select(c => c.Type).ToList();
            int accountId = Int32.Parse(claimsIdentity.Claims.Where(c => c.Type == "Id")
                   .Select(c => c.Value).SingleOrDefault());
            Student student=  _context.Students.SingleOrDefault(s => s.AccountId == accountId);
            return student;
        }
    }
}