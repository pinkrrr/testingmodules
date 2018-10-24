using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestingModule.Additional;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Controllers
{
    [CustomAuthorize(RoleName.Student)]
    public class StudentController : BaseController
    {
        private readonly QuizManager _quizManager;
        private readonly StudentPageHelper _studentPageHelper;

        public StudentController()
        {
            _quizManager = new QuizManager(Context);
            _studentPageHelper = new StudentPageHelper(Context);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _quizManager.Dispose();
                _studentPageHelper.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Student
        public async Task<ActionResult> Index()
        {
            //Check for active quiz
            //var active = studePageHelper.CheckActiveQuiz(User.Identity as System.Security.Claims.ClaimsIdentity);
            //if (active != null)
            //{
            //    return RedirectToAction("/" + active, "Quiz");
            //}
            //If no active quiz, or already answered
            var model = await _studentPageHelper.StudentsDisciplinesList();
            return View(model);
        }

        public async Task<ActionResult> StudentLectures(int disciplineId)
        {
            IList<Lecture> lect = new testingDbEntities().Lectures.Where(t => t.DisciplineId == disciplineId).ToList();
            IList<Discipline> disc = new testingDbEntities().Disciplines.ToList();
            LectureQuizViewModel model = new LectureQuizViewModel
            {
                Lectures = lect,
                Disciplines = disc,
                LecturesForQuizId = await _quizManager.GetQuizForLectureAlailability(lect.Select(l => l.DisciplineId).First())
            };
            return View(model);

        }

        public ActionResult StudentModules(int lectureId)
        {
            var discId = Context.Lectures.FirstOrDefault(t => t.Id == lectureId).DisciplineId;
            IList<Module> mod = Context.Modules.Where(t => t.LectureId == lectureId).ToList();
            IList<Lecture> lect = Context.Lectures.Where(t => t.DisciplineId == discId).ToList();
            ReasignViewModel test = new ReasignViewModel() { Lectures = lect, Modules = mod };
            return View(test);
        }
    }
}