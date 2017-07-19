using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using TestingModule.Additional;
using TestingModule.Models;
using TestingModule.ViewModels;

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
        public ActionResult NewDiscipline(Discipline model)
        {
            var result = new Adding().AddNewDiscipline(model.Name);
            return RedirectToAction("Disciplines");
        }
        public ActionResult EditDiscipline(Discipline model)
        {
            new Editing().EditDiscipline(model.Id, model.Name);
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
        public ActionResult NewLecture(Lecture model)
        {
            var result = new Adding().AddNewLecture(model.Name, model.DisciplineId);
            return RedirectToAction("Lectures");
        }
        public ActionResult EditLecture(Lecture model)
        {
            new Editing().EditLecture(model.Id, model.Name);
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
        public ActionResult NewModule(Module model)
        {
            var result = new Adding().AddNewModule(model.Name, model.LectureId, model.DisciplineId);
            return RedirectToAction("Modules");
        }
        public ActionResult EditModule(Module model)
        {
            new Editing().EditModule(model.Id, model.Name);
            return RedirectToAction("Modules");
        }
        public ActionResult DeleteModule(int moduleId)
        {
            new Deleting().DeleteModule(moduleId);
            return RedirectToAction("Modules");
        }


        //Question
        public ActionResult Questions(int moduleId)
        {
            testingDbEntities db = new testingDbEntities();
            
            var viewModels = (from q in db.Questions
                              join a in db.Answers on q.Id equals a.QuestionId
                              select new QueAns()
                              {
                                  DisciplineId = q.DisciplineId,
                                  LectureId = q.LectureId,
                                  ModuleId = q.ModuleId,
                                  QuestionId = q.Id,
                                  Question = q.Text,
                                  AnswerId = a.Id,
                                  Answer = a.Text
                              }).ToList();

            var neededQuestions = viewModels.Where(t => t.ModuleId == moduleId);
            return View(neededQuestions);
        }

        //Specialities
        public ActionResult Specialities()
        {
            List<Speciality> test = new testingDbEntities().Specialities.ToList();
            return View(test);
        }
        public ActionResult NewSpeciality(Speciality model)
        {
            var result = new Adding().AddNewSpeciality(model.Name);
            return RedirectToAction("Specialities");
        }
        public ActionResult EditSpeciality(Speciality model)
        {
            new Editing().EditSpeciality(model.Id, model.Name);
            return RedirectToAction("Specialities");
        }
        public ActionResult DeleteSpeciality(int specialityId)
        {
            new Deleting().DeleteSpeciality(specialityId);
            return RedirectToAction("Specialities");
        }



        //Groups
        public ActionResult Groups(int specialityId)
        {
            List<Group> test = new testingDbEntities().Groups.Where(t => t.SpecialityId == specialityId).ToList();
            return View(test);
        }
        public ActionResult NewGroup(Group model)
        {
            var result = new Adding().AddNewGroup(model.Name, model.SpecialityId);
            return RedirectToAction("Groups");
        }
        public ActionResult EditGroup(Group model)
        {
            new Editing().EditGroup(model.Id, model.Name);
            return RedirectToAction("Groups");
        }
        public ActionResult DeleteGroup(int groupId)
        {
            new Deleting().DeleteGroup(groupId);
            return RedirectToAction("Groups");
        }

        //Students
        public ActionResult Students(int groupId)
        {
            testingDbEntities db = new testingDbEntities();

            var viewModels = (from s in db.Students
                join a in db.Accounts on s.AccountId equals a.Id
                select new UserViewModel()
                {
                    Id = s.Id,
                    SpecialityId =  s.SpecialityId,
                    GroupId = s.GroupId,
                    AccountId = s.AccountId,
                    Login = a.Login,
                    Password = a.Password,
                    RoleId = a.RoleId
                }).ToList();

            var neededGroup = viewModels.Where(t => t.GroupId == groupId);
            return View(neededGroup);
        }
        public ActionResult NewStudent(Student model)
        {
            var result = new Adding().AddNewStudent(model.Name, model.Surname, model.GroupId, model.SpecialityId);
            return RedirectToAction("Students");
        }
        public ActionResult EditStudent(UserViewModel model)
        {
            new Editing().EditStudent(model.Id, model.Name, model.Surname, model.Surname, model.Password);
            return RedirectToAction("Students");
        }
        public ActionResult DeleteStudent(int studentId)
        {
            new Deleting().DeleteStudent(studentId);
            return RedirectToAction("Students");
        }

        //Lectors
        public ActionResult Lectors()
        {
            ViewBag.Message = "All lectors";
            return View();
        }
    }
}