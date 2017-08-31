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
        private testingDbEntities _context=new testingDbEntities();

        // GET: Quiz

        [Route("quiz/{moduleId}")]
        public async Task<ActionResult> Index()
        {
            //QuizViewModel qvm = await new QuizManager().GetQnA(moduleId,studentId);
            return View();
        }

        [HttpPost]
        public ActionResult RedirectToQuiz(QuizViewModel quizViewModel)
        {
            var redirectUrl=new UrlHelper(Request.RequestContext).Action("Index","Quiz",quizViewModel);
            return Json(new {Url=redirectUrl});
        }

        // GET: Statistic
        [Route("quiz/statistics/{moduleId}")]
        public async Task<ActionResult> Statistics(int moduleId)
        {
            IEnumerable<Question> question = await new QuizManager().GetQuestionsList(moduleId);
            return View(question);
        }
    }
}