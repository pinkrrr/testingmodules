using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestingModule.Models;
using TestingModule.ViewModels;
using TestingModule.Additional;
using System.Threading.Tasks;

namespace TestingModule.Controllers
{
    public class QuizController : Controller
    {
        private readonly testingDbEntities _context;

        public QuizController()
        {
            _context = new testingDbEntities();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        [Route("quiz/{moduleHistoryId}")]
        public async Task<ActionResult> Index(int moduleHistoryId)
        {
            QuizViewModel qvm = await new QuizManager().GetQnA(moduleHistoryId);
            if (qvm == null)
                return RedirectToAction("Index", "Student");
            if (qvm.Question == null)
                return View();
            return View(qvm);
        }

        //[HttpPost]
        //public ActionResult RedirectToQuiz(QuizViewModel quizViewModel)
        //{
        //    var redirectUrl=new UrlHelper(Request.RequestContext).Action("Index","Quiz",quizViewModel);
        //    return Json(new {Url=redirectUrl});
        //}

        // GET: Statistic

        [CustomAuthorize(Roles = RoleName.Lecturer)]
        [Route("quiz/modulestatistics/")]
        public async Task<ActionResult> ModuleStatistics()
        {
            Lector lector = await new AccountCredentials().GetLector();
            if (await _context.ModuleHistories.AnyAsync(mh => mh.StartTime != null
                                                         && mh.IsPassed == false
                                                         && mh.LectorId == lector.Id))
            {
                return View(await new QuizManager().GetRealTimeStatisticsViewModel(lector));
            }
            return RedirectToAction("index", "admin");

        }

        [Route("quiz/totalstatistics/")]
        public async Task<ActionResult> TotalStatistics()
        {
            return View(await new QuizManager().GetHistoriesForLector());
        }

        [Route("quiz/totalstatistics/history/{lectureHistoryId}")]
        public async Task<ActionResult> HistoryStatistics(int lectureHistoryId)
        {
            return View(await new QuizManager().GetModulesForLector(lectureHistoryId));
        }
    }
}