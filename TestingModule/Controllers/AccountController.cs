using System;
using System.Linq;
using System.Web.Mvc;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Controllers
{
    public class AccountController : Controller
    {
        private testingDbEntities _context = new testingDbEntities();

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
            account.Login = student.Username;
            account.Password = student.Pass;
            account.RoleId = 0;
            _context.Students.Add(student);
            _context.Accounts.Add(account);
            try
            {
                _context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

            return View("Default");
        }

        public RegistrationFormViewModel CreateRegistrationViewmodel()
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

    }


}