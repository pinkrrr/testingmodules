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
    public class QuizController : BaseController
    {
        private readonly QuizManager _quizManager;
        private readonly TimerAssociates _timerAssociates;

        public QuizController()
        {
            _quizManager = new QuizManager(Context);
            _timerAssociates = new TimerAssociates(Context);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
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
            if (await Context.ModuleHistories.AnyAsync(mh => mh.StartTime != null
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
            if (!Context.IndividualQuizPasseds.Any(itp => itp.Id == individualQuizId && itp.IsPassed == false))
            {
                return RedirectToAction("Index", "Student");
            }
            var model = await _quizManager.GetIndividualQnA(individualQuizId);
            var toSetStartDate = Context.IndividualQuizPasseds.SingleOrDefault(iq =>
                    iq.Id == individualQuizId && iq.IsPassed == false && iq.StartDate == null);
            if (toSetStartDate != null)
            {
                toSetStartDate.StartDate = DateTime.UtcNow;
                await Context.SaveChangesAsync();
            }
            _timerAssociates.StartTimer(individualQuizId, TimeSpan.FromMilliseconds(model.TimeLeft), TimerAssociates.TimerType.Individual);
            return View(model);
        }

        [Route("cumulativequiz/{cumulativeQuizId}")]
        public async Task<ActionResult> CumulativeQuiz(int cumulativeQuizId)
        {
            if (!Context.CumulativeQuizPasseds.Any(itp => itp.Id == cumulativeQuizId && itp.IsPassed == false))
            {
                return RedirectToAction("Index", "Student");
            }
            CumulativeQuizViewModel model = await _quizManager.GetCumulativeQnA(cumulativeQuizId);
            var toSetStartDate = Context.CumulativeQuizPasseds.SingleOrDefault(iq =>
                iq.Id == cumulativeQuizId && iq.IsPassed == false && iq.StartDate == null);
            if (toSetStartDate != null)
            {
                toSetStartDate.StartDate = DateTime.UtcNow;
                await Context.SaveChangesAsync();
            }
            _timerAssociates.StartTimer(cumulativeQuizId, TimeSpan.FromMilliseconds(model.TimeLeft), TimerAssociates.TimerType.Cumulative);
            return View(model);
        }

        [HttpGet]
        [Route("quiz/checkforactiveindividualquiz")]
        public async Task<JsonResult> CheckForActiveIndividualQuiz()
        {
            var studentId = AccountCredentials.GetStudentId();
            var individualQuizId = await Context.IndividualQuizPasseds
                .Where(iqp => iqp.StudentId == studentId && iqp.StartDate != null && iqp.EndDate == null)
                .OrderBy(o => o.StartDate).Select(s => s.Id).FirstOrDefaultAsync();
            return Json(individualQuizId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("quiz/checkforactivecumulativequiz")]
        public async Task<JsonResult> CheckForActiveCumulativeQuiz()
        {
            var studentId = AccountCredentials.GetStudentId();
            var cumulativeQuizId = await Context.CumulativeQuizPasseds
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
                await (from s in Context.Students
                       where s.Id == studentId
                       join lhg in Context.LectureHistoryGroups on s.GroupId equals lhg.GroupId
                       join sd in Context.StudentDisciplines on s.Id equals sd.StudentId
                       join lh in Context.LecturesHistories on sd.DisciplineId equals lh.DisciplineId
                       where lh.Id == lhg.LectureHistoryId
                       join mh in Context.ModuleHistories on lh.Id equals mh.LectureHistoryId
                       where mh.StartTime != null && mh.IsPassed == false
                       select mh).OrderBy(mh => mh.StartTime).Select(s => s.Id).FirstOrDefaultAsync();
            return Json(moduleHistoryId, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}