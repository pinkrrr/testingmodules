using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestingModule.Models;
using TestingModule.ViewModels;
using System.Security.Claims;
using System.Security.Principal;
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
        public static string GetRole()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value)
                   .SingleOrDefault();
        }

        public static async Task<Student> GetStudent()
        {
            testingDbEntities context = new testingDbEntities();
            var claimsIdentity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            int studentId = Convert.ToInt32(claimsIdentity.Identity.GetUserId());
            return await context.Students.SingleOrDefaultAsync(s => s.Id == studentId);
        }

        public static int GetStudentId()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return Convert.ToInt32(identity.Identity.GetUserId());

        }

        public static int GetStudentId(ClaimsIdentity identity)
        {
            return Convert.ToInt32(identity.GetUserId());
        }

        public static string GetStudentGroup(ClaimsIdentity identity)
        {
            return identity.Claims.Where(c => c.Type == "Group").Select(c => c.Value).SingleOrDefault();
        }

        public static string GetStudentGroup()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return identity.Claims.Where(c => c.Type == "Group").Select(c => c.Value).SingleOrDefault();
        }

        public static async Task<Lector> GetLector()
        {
            testingDbEntities context = new testingDbEntities();
            var claimsIdentity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var lectorId = Convert.ToInt32(claimsIdentity.Identity.GetUserId());
            Lector lector = await context.Lectors.SingleOrDefaultAsync(s => s.Id == lectorId);
            return lector;
        }

        public static bool AuthorizedAs(string role)
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return identity.Claims.Where(c => c.Type == "Group").Select(c => c.Value).SingleOrDefault() == role;
        }
    }
}
