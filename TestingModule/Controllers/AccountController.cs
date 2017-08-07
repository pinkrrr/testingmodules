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
            return View("Registration", registrationForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrationSave(Student student, Account account)
        {
            if (!ModelState.IsValid)
            {
                var registrationForm = CreateRegistrationViewmodel();
            }
            //account.Login = student.Username;
            //account.Password = student.Pass;
            account.RoleId = 2;
            _context.Students.Add(student);
            _context.Accounts.Add(account);
            _context.SaveChanges();
            return View("Index", "admin");
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
        [AllowAnonymous]
        public ActionResult Login()
        {
            var loginForm = new Account();
            return View("Login", loginForm);
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAttempt(Account account)
        {
            var loginForm = new Account();
            if (AccountValid(account.Login,account.Password))
            {
                //var accounts = _context.Accounts.ToList();
                var roles = _context.Roles.ToList();
                account = _context.Accounts.SingleOrDefault(a => a.Login == account.Login && a.Password == account.Password);
                if (account.RoleId != _context.Roles.Where(r => r.Name == "Student").Select(r => r.Id).SingleOrDefault())
                {
                    var ident = new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, account.Login),
                            new Claim(ClaimTypes.Name, account.Login),
                            new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                            new Claim(ClaimTypes.Role,roles.Where(r=>r.Id==account.RoleId).Select(r=>r.Name).SingleOrDefault())
                        },
                        DefaultAuthenticationTypes.ApplicationCookie);
                    HttpContext.GetOwinContext().Authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = false }, ident);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    Student student = _context.Students.SingleOrDefault(s=>s.AccountId==account.Id);
                    var ident = new ClaimsIdentity(
                    new[]
                        { 
                            new Claim(ClaimTypes.NameIdentifier, account.Login),
                            new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                            new Claim(ClaimTypes.Name,student.Name),
                            new Claim(ClaimTypes.Surname,student.Surname),
                            new Claim("Speciality", _context.Specialities.Where(sp=>sp.Id==student.SpecialityId).Select(sp=>sp.Name).SingleOrDefault()),
                            new Claim("Group",_context.Groups.Where(g=>g.Id==student.GroupId).Select(g=>g.Name).SingleOrDefault()),
                            new Claim(ClaimTypes.Role,"Student")
                        },
                        DefaultAuthenticationTypes.ApplicationCookie);
                    HttpContext.GetOwinContext().Authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = false }, ident);
                    return RedirectToAction("Index", "Student");
                }
                
            }
            // invalid username or password
            ModelState.AddModelError("", "invalid username or password");
            return View("Login", loginForm);
        }
        private bool AccountValid(string username, string password)
        {
            var accounts = _context.Accounts;
            return accounts.Any(a => a.Login == username && a.Password == password);
        }

        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "admin");
        }
    }
}