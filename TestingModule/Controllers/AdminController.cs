using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using TestingModule.Additional;
using TestingModule.Models;

namespace TestingModule.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Disciplines()
        {
            ViewBag.Message = "All disciplines";
            List<Discipline> test = new testingDbEntities().Disciplines.ToList();
            return View(test);

        }
        public ActionResult NewDiscipline(TestModel model)
        {
            var result = new Adding().AddNewDiscipline(model.Name);
            return RedirectToAction("Disciplines");
        }
        public ActionResult EditDiscipline(TestModel model)
        {
            var result = new Adding().AddNewDiscipline(model.Name);
            return RedirectToAction("Disciplines");
        }
        public ActionResult DeleteDiscipline(int disciplineId)
        {
            new Adding().DeleteDiscipline(disciplineId);
            return RedirectToAction("Disciplines");
        }



        public ActionResult Lectures(int disciplineId)
        {
            List<Lecture> test = new testingDbEntities().Lectures.Where(t => t.DisciplineId == disciplineId).ToList();
            return View(test);
        }
        public ActionResult Modules()
        {
            List<Module> test = new testingDbEntities().Modules.ToList();
            return View(test);
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


    }
}