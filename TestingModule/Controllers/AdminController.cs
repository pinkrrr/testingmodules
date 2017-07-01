using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
            return View("Disciplines");
        }
        public ActionResult EditDiscipline(TestModel model)
        {
            var result = new Adding().AddNewDiscipline(model.Name);
            return View("Disciplines");
        }
        public ActionResult DeleteDiscipline(TestModel model)
        {
            var result = new Adding().AddNewDiscipline(model.Name);
            return View("Disciplines");
        }



        public ActionResult Lectures(int disciplineId)
        {
            List<Lecture> test = new testingDbEntities().Lectures.Where(t => t.DisciplineId == disciplineId).ToList();
            return View(test);
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


    }
}