using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestingModule.Additional;
using TestingModule.Models;

namespace TestingModule.Controllers
{
    public class ErrorController : BaseController
    {
        private readonly Editing _editing;

        public ErrorController(ITestingDbEntityService context) : base(context)
        {
            _editing = new Editing(Context);
        }

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
            var test = Context.ExeptionLogs.Where(t => t.Resolved == false).ToList();
            return View(test);
        }
        public ActionResult Resolved(int exeptionId)
        {
            _editing.EditExeption(exeptionId);
            return RedirectToAction("Dashboard");
        }
    }
}