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
    public class StudentController : Controller
    {
        private testingDbEntities _db;
        private QuizManager _quizManager;

        public StudentController()
        {
            _db = new testingDbEntities();
            _quizManager=new QuizManager();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
                _quizManager = null;
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

        public async Task<ActionResult> StudentLectures(int disciplineId)
        {
            System.Diagnostics.Stopwatch watch=new Stopwatch();
            watch.Start();
            IList<Lecture> lect = new testingDbEntities().Lectures.Where(t => t.DisciplineId == disciplineId).ToList();
            IList<Discipline> disc = new testingDbEntities().Disciplines.ToList();
            LectureQuizViewModel model = new LectureQuizViewModel { Lectures = lect, Disciplines = disc, LecturesForQuizId = await _quizManager.GetQuizForLectureAlailability(lect.Select(l=>l.DisciplineId).First())};
            watch.Stop();
            var result = watch.ElapsedMilliseconds;
            return View(model);
            
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