using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class AccountCredentials : Controller
    {
        public string GetRole()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            return claimsIdentity.FindFirst(ClaimTypes.Role).Value;
        }

        public async Task<Student> GetStudent()
        {
            testingDbEntities _context=new testingDbEntities();
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int accountId = Int32.Parse(claimsIdentity.Claims.SingleOrDefault(c => c.Type == "Id").Value);
            Student student=  _context.Students.SingleOrDefault(s => s.AccountId == accountId);
            return student;
        }
    }
}