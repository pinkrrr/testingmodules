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
using TestingModule.Hubs;

namespace TestingModule.Controllers
{
    public class QuizController : Controller
    {
        private readonly testingDbEntities _context;
        private readonly QuizManager _quizManager;

        public QuizController()
        {
            _context = new testingDbEntities();
            _quizManager = new QuizManager();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
                _quizManager.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Realtime Testing

        [Route("quiz/{moduleHistoryId}")]
        public async Task<ActionResult> Index(int moduleHistoryId)
        {
            RealTimeQuizViewModel qvm = await _quizManager.GetRealtimeQnA(moduleHistoryId);
            if (qvm == null)
                return RedirectToAction("Index", "Student");
            if (qvm.Question == null)
                return View();
            QuizHub.Students.Add(qvm.ModuleHistoryId, qvm.Student.Id);
            return View(qvm);
        }

        [CustomAuthorize(Roles = RoleName.Lecturer)]
        [Route("quiz/modulestatistics/")]
        public async Task<ActionResult> ModuleStatistics()
        {
            Lector lector = await AccountCredentials.GetLector();
            if (await _context.ModuleHistories.AnyAsync(mh => mh.StartTime != null
                                                              && mh.IsPassed == false
                                                              && mh.LectorId == lector.Id))
            {
                return View(await _quizManager.GetRealTimeStatisticsViewModel(lector));
            }
            return RedirectToAction("index", "admin");

        }
        [CustomAuthorize(RoleName.Lecturer, RoleName.Administrator)]
        [Route("quiz/totalstatistics/")]
        public async Task<ActionResult> TotalStatistics()
        {
            return View(await _quizManager.GetHistoriesForLector());
        }

        [Route("quiz/totalstatistics/history/{lectureHistoryId}")]
        public async Task<ActionResult> HistoryStatistics(int lectureHistoryId)
        {
            return View(await _quizManager.GetModulesForLector(lectureHistoryId));
        }

        #endregion

        #region Individual Testing

        [Route("individualquiz/{individualQuizId}")]
        public async Task<ActionResult> IndividualQuiz(int individualQuizId)
        {
            if (!_context.IndividualQuizPasseds.Any(itp => itp.Id == individualQuizId && itp.IsPassed == false))
            {
                return RedirectToAction("Index", "Student");
            }
            var model = await _quizManager.GetIndividualQnA(individualQuizId);
            var toSetStartDate = _context.IndividualQuizPasseds.SingleOrDefault(iq =>
                    iq.Id == individualQuizId && iq.IsPassed == false && iq.StartDate == null);
            if (toSetStartDate != null)
            {
                toSetStartDate.StartDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            TimerAssociates.StartTimer(individualQuizId, TimeSpan.FromMilliseconds(model.TimeLeft), TimerAssociates.TimerType.IndividualId);
            return View(model);
        }

        [Route("cumulativequiz/{cumulativeQuizId}")]
        public async Task<ActionResult> CumulativeQuiz(int cumulativeQuizId)
        {
            if (!_context.CumulativeQuizPasseds.Any(itp => itp.Id == cumulativeQuizId && itp.IsPassed == false))
            {
                return RedirectToAction("Index", "Student");
            }
            CumulativeQuizViewModel model = await _quizManager.GetCumulativeQnA(cumulativeQuizId);
            var toSetStartDate = _context.CumulativeQuizPasseds.SingleOrDefault(iq =>
                iq.Id == cumulativeQuizId && iq.IsPassed == false && iq.StartDate == null);
            if (toSetStartDate != null)
            {
                toSetStartDate.StartDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            TimerAssociates.StartTimer(cumulativeQuizId, TimeSpan.FromMilliseconds(model.TimeLeft), TimerAssociates.TimerType.CumulativeId);
            return View(model);
        }

        [HttpGet]
        [Route("quiz/checkforactiveindividualquiz")]
        public async Task<JsonResult> CheckForActiveIndividualQuiz()
        {
            var studentId = AccountCredentials.GetStudentId();
            var individualQuizId = await _context.IndividualQuizPasseds
                .Where(iqp => iqp.StudentId == studentId && iqp.StartDate != null && iqp.EndDate == null)
                .OrderBy(o => o.StartDate).Select(s => s.Id).FirstOrDefaultAsync();
            return Json(individualQuizId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("quiz/checkforactivecumulativequiz")]
        public async Task<JsonResult> CheckForActiveCumulativeQuiz()
        {
            var studentId = AccountCredentials.GetStudentId();
            var cumulativeQuizId = await _context.CumulativeQuizPasseds
                .Where(iqp => iqp.StudentId == studentId && iqp.StartDate != null && iqp.EndDate == null)
                .OrderBy(o => o.StartDate).Select(s => s.Id).FirstOrDefaultAsync();
            return Json(cumulativeQuizId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("quiz/checkforactiverealtimequiz")]
        public async Task<JsonResult> CheckForActiveRealtimeQuiz()
        {
            var studentId = AccountCredentials.GetStudentId();
            var moduleHistoryId =
                await (from s in _context.Students
                       where s.Id == studentId
                       join lhg in _context.LectureHistoryGroups on s.GroupId equals lhg.GroupId
                       join sd in _context.StudentDisciplines on s.Id equals sd.StudentId
                       join lh in _context.LecturesHistories on sd.DisciplineId equals lh.DisciplineId
                       where lh.Id == lhg.LectureHistoryId
                       join mh in _context.ModuleHistories on lh.Id equals mh.LectureHistoryId
                       where mh.StartTime != null && mh.IsPassed == false
                       select mh).OrderBy(mh => mh.StartTime).Select(s => s.Id).FirstOrDefaultAsync();
            return Json(moduleHistoryId, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}