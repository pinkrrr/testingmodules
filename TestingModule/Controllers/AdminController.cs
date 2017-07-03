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

        //Discipline
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
            new Deleting().DeleteDiscipline(disciplineId);
            return RedirectToAction("Disciplines");
        }


        //Lecture
        public ActionResult Lectures(int disciplineId)
        {
            List<Lecture> test = new testingDbEntities().Lectures.Where(t => t.DisciplineId == disciplineId).ToList();
            return View(test);
        }
        public ActionResult NewLecture(TestModel model)
        {
            var result = new Adding().AddNewLecture(model.Name,  model.DisciplineId);
            return RedirectToAction("Lectures");
        }
        public ActionResult DeleteLecture(int lectureId)
        {
            new Deleting().DeleteLecture(lectureId);
            return RedirectToAction("Lectures");
        }



        //Module
        public ActionResult Modules(int lectureId)
        {
            List<Module> test = new testingDbEntities().Modules.Where(t => t.LectureId == lectureId).ToList();
            return View(test);
        }
        public ActionResult NewModule(TestModel model)
        {
            var result = new Adding().AddNewModule(model.Name, model.LectureId, model.DisciplineId);
            return RedirectToAction("Modules");
        }
        public ActionResult DeleteModule(int moduleId)
        {
            new Deleting().DeleteModule(moduleId);
            return RedirectToAction("Modules");
        }


        //Question
        public ActionResult Questions()
        {
            ViewBag.Message = "All questions from previously selected module";

            return View();
        }



    }
}