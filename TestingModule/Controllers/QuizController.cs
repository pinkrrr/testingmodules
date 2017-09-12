﻿using System;
using System.Collections.Generic;
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
        private testingDbEntities _context = new testingDbEntities();


        // GET: Quiz

        [Route("quiz/{moduleId}")]
        public async Task<ActionResult> Index(int moduleId)
        {
            QuizViewModel qvm = await new QuizManager().GetQnA(moduleId);
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
        [Route("quiz/modulestatistics/{moduleId}")]
        public async Task<ActionResult> ModuleStatistics(int moduleId)
        {
            IEnumerable<Question> question = await new QuizManager().GetQuestionsList(moduleId);
            return View(question);
        }

        [Route("quiz/totalstatistics/")]
        public async Task<ActionResult> TotalStatistics()
        {
            return View(await new QuizManager().GetHistorieForLector());
        }

        [Route("quiz/totalstatistics/history/{lectureHistoryId}")]
        public async Task<ActionResult> HistoryStatistics(int lectureHistoryId)
        {
            return View(await new QuizManager().GetModulesForLector(lectureHistoryId));
        }
    }
}