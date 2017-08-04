using System;
using System.Collections;
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
                // ignored
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
            var db = new testingDbEntities();
            var discId = db.Lectures.FirstOrDefault(t => t.Id == lectureId).DisciplineId;
            IEnumerable<Module> mod = db.Modules.Where(t => t.LectureId == lectureId).ToList();
            IEnumerable<Lecture> lect = db.Lectures.Where(t => t.DisciplineId == discId).ToList();
            ReasignViewModel test = new ReasignViewModel() { Lectures = lect, Modules = mod };
            return View(test);
        }
        public ActionResult NewModule(Module model)
        {
            try
            {
                new Adding().AddNewModule(model.Name.TrimEnd().TrimStart(), model.LectureId, model.DisciplineId);
            }
            catch
            {
                // ignored
            }
            return RedirectToAction("Modules");
        }
        public ActionResult EditModule(Module model)
        {
            new Editing().EditModule(model.Id, model.Name.TrimEnd().TrimStart(), model.LectureId);
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
            var lectId = db.Modules.FirstOrDefault(t => t.Id == moduleId).LectureId;
            IEnumerable<Module> mod = db.Modules.Where(t => t.LectureId == lectId).ToList();
            foreach (var model in viewModels)
            {
                model.Modules = mod;
            }
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
            new Editing().EditQuestion(model.QuestionId, model.Question.TrimEnd().TrimStart(), model.ModuleId);
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
                new Editing().EditQuestion(model.FirstOrDefault().QuestionId, model.FirstOrDefault().Question.TrimEnd().TrimStart()
                    , model.FirstOrDefault().ModuleId);
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
            var db = new testingDbEntities();
            IEnumerable<Group> grp = db.Groups.Where(t => t.SpecialityId == specialityId).ToList();
            IEnumerable<Speciality> spc = db.Specialities.ToList();
            ReasignViewModel test = new ReasignViewModel() { Groups = grp, Specialities = spc };
            return View(test);
        }
        public ActionResult NewGroup(Group model)
        {
            try
            {
                new Adding().AddNewGroup(model.Name.TrimEnd().TrimStart(), model.SpecialityId);
            }
            catch
            {
                // ignored
            }
            return RedirectToAction("Groups");
        }
        public ActionResult EditGroup(Group model)
        {
            new Editing().EditGroup(model.Id, model.Name.TrimEnd().TrimStart(), model.SpecialityId);
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
            var db = new testingDbEntities();
            var specId = db.Groups.FirstOrDefault(t => t.Id == groupId).SpecialityId;
            List<int> accList = new List<int>();
            IEnumerable<Student> std = db.Students.Where(t => t.GroupId == groupId).ToList();
            foreach (var student in std)
            {
                accList.Add(student.AccountId);
            }
            IEnumerable<Account> acc = db.Accounts.Where(t => accList.Contains(t.Id)).ToList();
            IEnumerable<Group> grp = db.Groups.Where(t => t.SpecialityId == specId).ToList();
            ReasignViewModel test = new ReasignViewModel() { Groups = grp, Accounts = acc, Students = std };
            return View(test);
        }
        public ActionResult NewStudent(Student model)
        {
            try
            {
                new Adding().AddNewStudent(model.Name.TrimEnd().TrimStart(), model.Surname.TrimEnd().TrimStart(), model.GroupId, model.SpecialityId);
            }
            catch (Exception)
            {
                // ignored
            }
            return RedirectToAction("Students");
        }
        public ActionResult EditStudent(UserViewModel model)
        {
            new Editing().EditStudent(model.Id, model.Name.TrimEnd().TrimStart(), model.Surname.TrimEnd().TrimStart(), model.Login, model.Password, model.GroupId);
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

        //DisciplineStudents
        public ActionResult DisciplineStudents(int disciplineId)
        {
            var db = new testingDbEntities();
            IEnumerable<Group> grp = db.Groups.ToList();
            IEnumerable<Speciality> spc = db.Specialities.ToList();
            IEnumerable<Student> std = db.Students.ToList();
            IList<StudentDiscipline> studDisc = db.StudentDisciplines.Where(t => t.DisciplineId == disciplineId).ToList();
            IEnumerable<Discipline> disc = db.Disciplines.Where(t => t.Id == disciplineId).ToList();
            foreach (var stdc in std)
            {
                if (studDisc.All(t => t.StudentId != stdc.Id))
                {
                    studDisc.Add(new StudentDiscipline()
                    {
                        Id = 666,
                        StudentId = stdc.Id,
                        DisciplineId = disciplineId,
                        IsSelected = false
                    });
                }
                else
                {
                    try
                    {
                        studDisc.FirstOrDefault(t => t.StudentId == stdc.Id && t.DisciplineId == disciplineId).IsSelected =
                            true;
                    }
                    catch
                    {
                        studDisc.Add(new StudentDiscipline()
                        {
                            Id = 666,
                            StudentId = stdc.Id,
                            DisciplineId = disciplineId,
                            IsSelected = false
                        });
                    }
                }
            }
            ReasignViewModel test = new ReasignViewModel() { Groups = grp, Specialities = spc, Students = std, Disciplines = disc, StudentDisciplines = studDisc };
            return View(test);
        }
        public ActionResult NewStudentConnections(ReasignViewModel model)
        {
            new Adding().AddNewStudentConnection(model);
            return RedirectToAction("DisciplineStudents");
        }
    }
}