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

        #region Realtime Testing

        [Route("quiz/{moduleHistoryId}")]
        public async Task<ActionResult> Index(int moduleHistoryId)
        {
            RealTimeQuizViewModel qvm = await new QuizManager().GetRealtimeQnA(moduleHistoryId);
            if (qvm == null)
                return RedirectToAction("Index", "Student");
            if (qvm.Question == null)
                return View();
            return View(qvm);
        }

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

        #endregion

        #region Individual Testing

        [Route("individualquiz/{individualQuizId}")]
        public async Task<ActionResult> IndividualQuiz(int individualQuizId)
        {
            var studentId = new AccountCredentials().GetStudentId();
            if (!_context.IndividualQuizPasseds.Any(itp => itp.Id == individualQuizId && itp.StudentId == studentId && itp.IsPassed == false))
            {
                return RedirectToAction("Index", "Student");
            }
            var temp = await new QuizManager().GetIndividualQnA(individualQuizId);
            return View();
        }

        #endregion
    }
}