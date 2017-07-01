using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestingModule.Models;

namespace TestingModule.Controllers
{
    public class AdminController : Controller
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Disciplines()
        {
            ViewBag.Message = "All disciplines";
            
            return View();
        }

        public ActionResult Lectures()
        {
            ViewBag.Message = "All lectures from previously selected discipline";
            
            return View();
        }
        public ActionResult Modules()
        {
            ViewBag.Message = "All modules from previously selected lecture";

            return View();
        }
        public ActionResult Questions()
        {
            ViewBag.Message = "All questions from previously selected module";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult NewLecture()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}