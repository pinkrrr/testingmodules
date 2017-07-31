using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
            var db = new testingDbEntities();
            ViewBag.Message = "All disciplines";
            IEnumerable<Lector> lectors = db.Lectors.ToList();
            List<DiscLecotorViewModel> viewModels = db.Disciplines.Select(d => new DiscLecotorViewModel
            {
                Id = d.Id,
                Name = d.Name
            }
            ).ToList();
            foreach (var model in viewModels)
            {
                model.Lectors = lectors;
                model.LectorId = db.LectorDisciplines.Where(t => model.Id == t.DisciplineId).Select(t => t.LectorId)
                    .FirstOrDefault();
            }
            return View(viewModels);
        }
        public ActionResult NewDiscipline(DiscLecotorViewModel model)
        {
            if (model.LectorId != null)
            {
                new Adding().AddNewDiscipline(model.Name.TrimEnd().TrimStart(), model.LectorId);
            }
            return RedirectToAction("Disciplines");
        }
        public ActionResult EditDiscipline(DiscLecotorViewModel model)
        {
            if (model.LectorId != null)
            {
                new Editing().EditDiscipline(model.Id, model.Name.TrimEnd().TrimStart(), model.LectorId);
            }

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
            IEnumerable<Lecture> lect = new testingDbEntities().Lectures.Where(t => t.DisciplineId == disciplineId).ToList();
            IEnumerable<Discipline> disc = new testingDbEntities().Disciplines.ToList();
            ReasignViewModel test = new ReasignViewModel() { Lectures = lect, Disciplines = disc };
            return View(test);
        }
        public ActionResult NewLecture(Lecture model)
        {
            new Adding().AddNewLecture(model.Name.TrimEnd().TrimStart(), model.DisciplineId);
            return RedirectToAction("Lectures");
        }
        public ActionResult EditLecture(Lecture model)
        {
            try
            {
                new Editing().EditLecture(model.Id, model.Name.TrimEnd().TrimStart(), model.DisciplineId);
            }
            catch
            {
            }
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
            new Adding().AddNewModule(model.Name.TrimEnd().TrimStart(), model.LectureId, model.DisciplineId);
            return RedirectToAction("Modules");
        }
        public ActionResult EditModule(Module model)
        {
            new Editing().EditModule(model.Id, model.Name.TrimEnd().TrimStart());
            return RedirectToAction("Modules");
        }
        public ActionResult DeleteModule(int moduleId)
        {
            new Deleting().DeleteModule(moduleId);
            return RedirectToAction("Modules");
        }


        //Question
        public ActionResult Questions(int? moduleId)
        {
            testingDbEntities db = new testingDbEntities();

            List<QueAns> viewModels = (from q in db.Questions
                                       from a in db.Answers.Where(t => q.Id == t.QuestionId).DefaultIfEmpty()
                                       select new QueAns()
                                       {
                                           DisciplineId = q.DisciplineId,
                                           LectureId = q.LectureId,
                                           ModuleId = q.ModuleId,
                                           QuestionId = q.Id,
                                           Question = q.Text,
                                           AnswerId = a.Id,
                                           Answer = a.Text,
                                           IsCorrect = a.IsCorrect
                                       }).ToList();

            var neededQuestions = viewModels.Where(t => t.ModuleId == moduleId).ToList();
            return View(neededQuestions);
        }
        public ActionResult NewQuestion(QueAns model)
        {
            new Adding().AddNewQuestion(model.Question.TrimEnd().TrimStart(), model.LectureId, model.DisciplineId, model.ModuleId);
            return RedirectToAction("Questions");
        }
        public ActionResult EditQuestion(QueAns model)
        {
            new Editing().EditQuestion(model.QuestionId, model.Question.TrimEnd().TrimStart());
            return RedirectToAction("Questions");
        }
        public ActionResult DeleteQuestion(int questionId)
        {
            new Deleting().DeleteQuestion(questionId);
            return RedirectToAction("Questions");
        }
        public ActionResult NewAnswer(QueAns model)
        {
            new Adding().AddNewAnswer(model.Answer.TrimEnd().TrimStart(), model.QuestionId);
            return RedirectToAction("Questions");
        }
        public ActionResult EditAnswer(List<QueAns> model)
        {
            if (model != null)
            {
                foreach (var item in model)
                {
                    new Editing().EditAnswer(item.AnswerId, item.Answer.TrimEnd().TrimStart(), item.IsCorrect);
                }
                new Editing().EditQuestion(model.FirstOrDefault().QuestionId, model.FirstOrDefault().Question.TrimEnd().TrimStart());
            }
            return RedirectToAction("Questions");
        }
        public ActionResult DeleteAnswer(int answerId)
        {
            new Deleting().DeleteAnswer(answerId);
            return RedirectToAction("Questions");
        }

        //Specialities
        public ActionResult Specialities()
        {
            List<Speciality> test = new testingDbEntities().Specialities.ToList();
            return View(test);
        }
        public ActionResult NewSpeciality(Speciality model)
        {
            new Adding().AddNewSpeciality(model.Name.TrimEnd().TrimStart());
            return RedirectToAction("Specialities");
        }
        public ActionResult EditSpeciality(Speciality model)
        {
            new Editing().EditSpeciality(model.Id, model.Name.TrimEnd().TrimStart());
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
            new Adding().AddNewGroup(model.Name.TrimEnd().TrimStart(), model.SpecialityId);
            return RedirectToAction("Groups");
        }
        public ActionResult EditGroup(Group model)
        {
            new Editing().EditGroup(model.Id, model.Name.TrimEnd().TrimStart());
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
                                  SpecialityId = s.SpecialityId,
                                  GroupId = s.GroupId,
                                  AccountId = s.AccountId,
                                  Name = s.Name,
                                  Surname = s.Surname,
                                  Login = a.Login,
                                  Password = a.Password,
                                  RoleId = a.RoleId
                              }).ToList();

            var neededGroup = viewModels.Where(t => t.GroupId == groupId);
            return View(neededGroup);
        }
        public ActionResult NewStudent(Student model)
        {
            new Adding().AddNewStudent(model.Name.TrimEnd().TrimStart(), model.Surname.TrimEnd().TrimStart(), model.GroupId, model.SpecialityId);
            return RedirectToAction("Students");
        }
        public ActionResult EditStudent(UserViewModel model)
        {
            new Editing().EditStudent(model.Id, model.Name.TrimEnd().TrimStart(), model.Surname.TrimEnd().TrimStart(), model.Login, model.Password);
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
            testingDbEntities db = new testingDbEntities();

            var viewModels = (from l in db.Lectors
                              join a in db.Accounts on l.AccountId equals a.Id
                              select new UserViewModel()
                              {
                                  Id = l.Id,
                                  Name = l.Name,
                                  Surname = l.Surname,
                                  Login = a.Login,
                                  Password = a.Password,
                                  RoleId = a.RoleId
                              }).ToList();

            return View(viewModels);
        }
        public ActionResult NewLector(Lector model)
        {
            new Adding().AddNewLector(model.Name.TrimEnd().TrimStart(), model.Surname);
            return RedirectToAction("Lectors");
        }
        public ActionResult EditLector(UserViewModel model)
        {
            new Editing().EditLector(model.Id, model.Name.TrimEnd().TrimStart(), model.Surname.TrimEnd().TrimStart(), model.Login, model.Password);
            return RedirectToAction("Lectors");
        }
        public ActionResult DeleteLector(int lectorId)
        {
            new Deleting().DeleteLector(lectorId);
            return RedirectToAction("Lectors");
        }
    }
}