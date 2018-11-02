using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestingModule.Additional;
using TestingModule.Models;

namespace TestingModule.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult ServerError()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            var test = new List<ExeptionLog>();
            test = new testingDbEntities().ExeptionLogs.Where(t => t.Resolved == false).ToList();
            return View(test);
        }
        public ActionResult Resolved(int exeptionId)
        {
            new Editing().EditExeption(exeptionId);
            return RedirectToAction("Dashboard");
        }
    }
}