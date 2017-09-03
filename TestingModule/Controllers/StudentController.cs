using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TestingModule.Additional;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index()
        {
            var studePageHelper = new StudentPageHelper();
            //Check for active quiz
            //var active = studePageHelper.CheckActiveQuiz(User.Identity as System.Security.Claims.ClaimsIdentity);
            //if (active != null)
            //{
            //    return RedirectToAction("/" + active, "Quiz");
            //}
            //If no active quiz, or already answered
            var viewModels =
                studePageHelper.StudentsDisciplinesList(User.Identity as System.Security.Claims.ClaimsIdentity);
            return View(viewModels);
        }
    }
}