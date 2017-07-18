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

namespace TestingModule.Controllers
{
    public class AccountController : Controller
    {
        private testingDbEntities _context = new testingDbEntities();
       
        //~Account/Registration

        public ActionResult Registration()
        {
            var registrationForm = CreateRegistrationViewmodel();
            return View("Registration",registrationForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrationSave(Student student, Account account)
        {
            if(!ModelState.IsValid)
            {
                var registrationForm=CreateRegistrationViewmodel();
            }
            account.Login = student.Username;
            account.Password = student.Pass;
            account.RoleId = 2;
            _context.Students.Add(student);
            _context.Accounts.Add(account);
            _context.SaveChanges();
            return View("Index","Admin");
        }

        private RegistrationFormViewModel CreateRegistrationViewmodel()
        {
            var groups = _context.Groups.ToList();
            var specialities = _context.Specialities.ToList();
            var roles = _context.Roles.ToList();
            var registrationForm = new RegistrationFormViewModel()
            {
                Account = new Account(),
                Student = new Student(),
                Groups = groups,
                Specialities = specialities,
                Roles = roles
                };
            return registrationForm;
        }

        //~Account/Login

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (AccountValid(username, password))
            {
                var accounts = _context.Accounts;
                var roles = _context.Roles.ToList();
                Account account = accounts.Single(a => a.Login == username && a.Password == password);

                if (account.RoleId == roles.Where(r => r.Name == "Administrator").Select(r => r.Id).Single())
                {

                }
                else
                {
                    Student student; 
                    var ident = new ClaimsIdentity(
                      new[] { 
              // adding following 2 claim just for supporting default antiforgery provider
              new Claim(ClaimTypes.NameIdentifier, username),
              new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

              new Claim(ClaimTypes.Name,username),
              new Claim(ClaimTypes.Surname,username),

              // optionally you could add roles if any
              new Claim("Group","RoleName"),
              new Claim(ClaimTypes.Role, "AnotherRole"),

                      },
                      DefaultAuthenticationTypes.ApplicationCookie);
                    HttpContext.GetOwinContext().Authentication.SignIn(
                       new AuthenticationProperties { IsPersistent = false }, ident);
                }
                return RedirectToAction("MyAction"); // auth succeed 
            }
            // invalid username or password
            ModelState.AddModelError("", "invalid username or password");
            return View();
        }

        private bool AccountValid(string username, string password)
        {
            var accounts = _context.Accounts;
            return accounts.Any(a => a.Login == username && a.Password == password);
        }
    }
}