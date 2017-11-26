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
        private testingDbEntities _db;

        public StudentController()
        {
            _db = new testingDbEntities();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

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

        public ActionResult StudentLectures(int disciplineId)
        {
            IList<Lecture> lect = new testingDbEntities().Lectures.Where(t => t.DisciplineId == disciplineId).ToList();
            IList<Discipline> disc = new testingDbEntities().Disciplines.ToList();
            ReasignViewModel test = new ReasignViewModel() { Lectures = lect, Disciplines = disc };
            return View(test);
        }

        public ActionResult StudentModules(int lectureId)
        {
            var discId = _db.Lectures.FirstOrDefault(t => t.Id == lectureId).DisciplineId;
            IList<Module> mod = _db.Modules.Where(t => t.LectureId == lectureId).ToList();
            IList<Lecture> lect = _db.Lectures.Where(t => t.DisciplineId == discId).ToList();
            ReasignViewModel test = new ReasignViewModel() { Lectures = lect, Modules = mod };
            return View(test);
        }
    }
}