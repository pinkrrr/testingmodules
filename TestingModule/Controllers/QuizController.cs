using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestingModule.Models;

namespace TestingModule.Controllers
{
    public class QuizController : Controller
    {
        private testingDbEntities _context=new testingDbEntities();
        // GET: Quiz
        public ActionResult Index()
        {
            return View();
        }
    }
}