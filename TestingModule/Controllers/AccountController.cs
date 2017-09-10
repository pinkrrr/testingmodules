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

namespace TestingModule.Controllers
{
    public class AccountController : Controller
    {
        private testingDbEntities _context = new testingDbEntities();

        #region Registration
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
            return View("Index", "Admin");
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
        #endregion

        #region Login

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToHome();
            }
            var loginForm = new Account();
            return View(loginForm);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAttempt(Account account)
        {
            if (AccountValid(account.Login, account.Password))
            {
                var roles = _context.Roles.ToList();
                ClaimsIdentity identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                account = _context.Accounts.SingleOrDefault(a => a.Login == account.Login && a.Password == account.Password);
                identity.AddClaims(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, account.Login),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                    new Claim(ClaimTypes.Role,roles.Where(r=>r.Id==account.RoleId).Select(r=>r.Name).SingleOrDefault()),
                    new Claim("Id",account.Id.ToString())
                });
                if (account.RoleId == _context.Roles.Where(r => r.Name == RoleName.Lecturer).Select(r => r.Id).SingleOrDefault())
                {
                    Lector lector = _context.Lectors.SingleOrDefault(s => s.AccountId == account.Id);
                    identity.AddClaims(new[]
                    {
                        new Claim(ClaimTypes.Name,lector.Name),
                        new Claim(ClaimTypes.Surname,lector.Surname),
                    });
                }
                if (account.RoleId == _context.Roles.Where(r => r.Name == RoleName.Student).Select(r => r.Id).SingleOrDefault())
                {
                    Student student = _context.Students.SingleOrDefault(s => s.AccountId == account.Id);
                    identity.AddClaims(new[]
                    {
                        new Claim(ClaimTypes.Name, student.Name),
                        new Claim(ClaimTypes.Surname, student.Surname),
                        new Claim("Speciality", _context.Specialities.Where(sp => sp.Id == student.SpecialityId).Select(sp => sp.Name).SingleOrDefault()),
                        new Claim("Group", _context.Groups.Where(g => g.Id == student.GroupId).Select(g => g.Name).SingleOrDefault()),
                    });
                }
                HttpContext.GetOwinContext().Authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = false }, identity);
                return RedirectToAction("Login");

            }
            TempData["FailLogin"] = "Неправильний логін, або пароль! Спробуйте ще раз.";
            return RedirectToAction("Login");
        }

        private bool AccountValid(string username, string password)
        {
            var accounts = _context.Accounts;
            return accounts.Any(a => a.Login == username && a.Password == password);
        }

        public ActionResult RedirectToHome()
        {

            var role = new AccountCredentials().GetRole();
            if (role == RoleName.Student)
            {
                return RedirectToAction("Index", "Student");
            }
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("login", "account");
        }
    }
    #endregion

    public class CustomAuthorize : AuthorizeAttribute
    {
        public CustomAuthorize(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Error", action = "NotFound" }));
            }
        }
    }
}



