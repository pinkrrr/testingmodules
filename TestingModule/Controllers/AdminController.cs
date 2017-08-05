using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using Microsoft.Office.Interop.Excel;
using TestingModule.Additional;
using TestingModule.Models;
using TestingModule.ViewModels;
using Module = TestingModule.Models.Module;

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
            try
            {
                if (model.LectorId != null)
                {
                    new Adding().AddNewDiscipline(model.Name.TrimEnd().TrimStart(), model.LectorId);
                    TempData["Success"] = "Дисципліна - \"" + model.Name.TrimEnd().TrimStart() + "\" була успішно додана!";
                }
            }
            catch (Exception)
            {
            }

            return RedirectToAction("Disciplines");
        }
        public ActionResult EditDiscipline(DiscLecotorViewModel model)
        {
            try
            {
                if (model.LectorId != null)
                {
                    new Editing().EditDiscipline(model.Id, model.Name.TrimEnd().TrimStart(), model.LectorId);
                    TempData["Success"] = "Зміни було успішно збережено";
                }
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Disciplines");
        }
        public ActionResult DeleteDiscipline(int disciplineId)
        {
            try
            {
                new Deleting().DeleteDiscipline(disciplineId);
                TempData["Success"] = "Дисципліна - \"" + disciplineId + "\" була успішно видалена!";
            }
            catch (Exception)
            {

            }

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
            try
            {
                new Adding().AddNewLecture(model.Name.TrimEnd().TrimStart(), model.DisciplineId);
                TempData["Success"] = "Лекція - \"" + model.Name.TrimEnd().TrimStart() + "\" була успішно додана!";
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Lectures");
        }
        public ActionResult EditLecture(Lecture model)
        {
            try
            {
                new Editing().EditLecture(model.Id, model.Name.TrimEnd().TrimStart(), model.DisciplineId);
                TempData["Success"] = "Зміни було успішно збережено!";
            }
            catch
            {
                // ignored
            }
            return RedirectToAction("Lectures");
        }
        public ActionResult DeleteLecture(int lectureId)
        {
            try
            {
                new Deleting().DeleteLecture(lectureId);
                TempData["Success"] = "Лекція була успішно видалена!";
            }
            catch (Exception)
            {

            }
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
                TempData["Success"] = "Модуль - \"" + model.Name.TrimEnd().TrimStart() + "\" був успішно доданий!";
            }
            catch
            {
                // ignored
            }
            return RedirectToAction("Modules");
        }
        public ActionResult EditModule(Module model)
        {
            try
            {
                new Editing().EditModule(model.Id, model.Name.TrimEnd().TrimStart(), model.LectureId);
                TempData["Success"] = "Зміни було успіщно збережено!";
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Modules");
        }
        public ActionResult DeleteModule(int moduleId)
        {
            try
            {
                new Deleting().DeleteModule(moduleId);
                TempData["Success"] = "Модуль був успішно видалений!";
            }
            catch (Exception)
            {

            }
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
            try
            {
                new Adding().AddNewQuestion(model.Question.TrimEnd().TrimStart(), model.LectureId, model.DisciplineId, model.ModuleId);
                TempData["Success"] = "Питання - \"" + model.Question.TrimEnd().TrimStart() + "\" було успішно додано!";
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Questions");
        }
        public ActionResult EditQuestion(QueAns model)
        {
            try
            {
                new Editing().EditQuestion(model.QuestionId, model.Question.TrimEnd().TrimStart(), model.ModuleId);
                TempData["Success"] = "Зміни було збережено!";
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Questions");
        }
        public ActionResult DeleteQuestion(int questionId)
        {
            try
            {
                new Deleting().DeleteQuestion(questionId);
                TempData["Success"] = "Модуль був успішно видалений!";
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Questions");
        }
        public ActionResult NewAnswer(QueAns model)
        {
            try
            {
                new Adding().AddNewAnswer(model.Answer.TrimEnd().TrimStart(), model.QuestionId);
                TempData["Success"] = "Відповідь - \"" + model.Answer.TrimEnd().TrimStart() + "\" була успішно додана!";
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Questions");
        }
        public ActionResult EditAnswer(List<QueAns> model)
        {
            try
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
                TempData["Success"] = "Модуль був успішно видалений!";
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Questions");
        }
        public ActionResult DeleteAnswer(int answerId)
        {
            try
            {
                new Deleting().DeleteAnswer(answerId);
                TempData["Success"] = "Відповідь була успішно видалена!";
            }
            catch (Exception)
            {
            }
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
            try
            {
                new Adding().AddNewSpeciality(model.Name.TrimEnd().TrimStart());
                TempData["Success"] = "Спеціальність - \"" + model.Name.TrimEnd().TrimStart() + "\" була успішно додана!";
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Specialities");
        }
        public ActionResult EditSpeciality(Speciality model)
        {
            try
            {
                new Editing().EditSpeciality(model.Id, model.Name.TrimEnd().TrimStart());
                TempData["Success"] = "Зміни було успішно збережено!";
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Specialities");
        }
        public ActionResult DeleteSpeciality(int specialityId)
        {
            try
            {
                new Deleting().DeleteSpeciality(specialityId);
                TempData["Success"] = "Спеціальність була успішно видалена!";
            }
            catch (Exception)
            {
            }
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
                TempData["Success"] = "Група - \"" + model.Name.TrimEnd().TrimStart() + "\" була успішно додана!";
            }
            catch
            {
                // ignored
            }
            return RedirectToAction("Groups");
        }
        public ActionResult EditGroup(Group model)
        {
            try
            {
                new Editing().EditGroup(model.Id, model.Name.TrimEnd().TrimStart(), model.SpecialityId);
                TempData["Success"] = "Зміни було успішно збережено!";
            }
            catch (Exception )
            {
            }
            return RedirectToAction("Groups");
        }
        public ActionResult DeleteGroup(int groupId)
        {
            try
            {
                new Deleting().DeleteGroup(groupId);
                TempData["Success"] = "Групу було успішно видалено!";
            }
            catch (Exception)
            {
            }
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
                TempData["Success"] = "Студент - \"" + model.Name.TrimEnd().TrimStart() +" " + model.Surname.TrimEnd().TrimStart() + "\" був успішно доданий!";
            }
            catch (Exception)
            {
                // ignored
            }
            return RedirectToAction("Students");
        }
        public ActionResult DownloadStudentExcel(int groupId)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug", "") + @"\Temp");
                foreach (FileInfo item in di.GetFiles())
                {
                    item.Delete();
                }
            }
            catch (Exception e)
            {

            }
            var db = new testingDbEntities();
            var group = db.Groups.FirstOrDefault(t => t.Id == groupId).Name;
            var students = db.Students.Where(t => t.GroupId == groupId).ToList();
            var account = db.Accounts.ToList();
            var name = group + ".xls";
            var path = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug", "") + @"\Temp\" + name;
            var file = File(path, System.Net.Mime.MediaTypeNames.Application.Octet, name);
            Application Inte;
            _Workbook intbook;
            _Worksheet intsheet;
            Inte = new Application();
            intbook = Inte.Workbooks.Open(AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug", "") + @"\Templates\studentsTemplate.xlsx");
            try
            {
                intsheet = (_Worksheet)Inte.ActiveSheet;
                intsheet.Name = DateTime.Now.ToString(group);
                intsheet.Cells[1, 1] = group;
                foreach (var student in students)
                {
                    intsheet.Cells[3 + students.IndexOf(student), 1] = student.Name;
                    intsheet.Cells[3 + students.IndexOf(student), 2] = student.Surname;
                    intsheet.Cells[3 + students.IndexOf(student), 3] = student.Id;
                    intsheet.Cells[3 + students.IndexOf(student), 4] = account.FirstOrDefault(t => t.Id == student.AccountId).Login;
                    intsheet.Cells[3 + students.IndexOf(student), 5] = account.FirstOrDefault(t => t.Id == student.AccountId).Password;
                }
                intsheet.Columns.AutoFit();
                intsheet.SaveAs(path);
                intbook.Close();
            }
            catch (Exception)
            {
                intbook.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                Inte.Quit();
            }
            return File(path, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
        [HttpPost]
        public ActionResult UploadStudentExcel(int groupId, int specialityId)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug", "") + @"\Temp");
                foreach (FileInfo item in di.GetFiles())
                {
                    item.Delete();
                }
            }
            catch (Exception)
            {
            }

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName.Replace(".xls", "").Replace(".xlsx", "") + DateTime.Now.ToString("MM_dd_yyyy_H_mm_ss") + ".xls");
                    var path = Path.Combine(Server.MapPath("~/Temp/"), fileName);
                    file.SaveAs(path);
                    Application Inte;
                    _Workbook intbook;
                    _Worksheet intsheet;
                    Inte = new Application();
                    intbook = Inte.Workbooks.Open(path);
                    intsheet = (_Worksheet)Inte.ActiveSheet;
                    try
                    {
                        var count = intsheet.Cells[1, 8].Value;
                        var students = new testingDbEntities().Students.Where(t => t.GroupId == groupId && t.SpecialityId == specialityId).ToList();
                        var accounts = new testingDbEntities().Accounts.ToList();
                        List<int?> ids = new ListStack<int?>();
                        for (int i = 0; i < count; i++)
                        {
                            var name = intsheet.Cells[3 + i, 1].Value.ToString();
                            var surname = intsheet.Cells[3 + i, 2].Value.ToString();
                            int? studId = (int?)intsheet.Cells[3 + i, 3].Value;
                            String login = intsheet.Cells[3 + i, 4].Value;
                            String password = intsheet.Cells[3 + i, 5].Value;
                            ids.Add(studId);
                            if (!students.Any(t => t.Id == studId))
                            {
                                new Adding().AddNewStudent(name.TrimEnd().TrimStart(), surname.TrimEnd().TrimStart(), groupId, specialityId);
                            }
                            else
                            {
                                var stud = students.FirstOrDefault(t => t.Id == studId);
                                var acc = accounts.FirstOrDefault(t => t.Id == stud.AccountId);
                                if (stud.Name != name || stud.Surname != surname || acc.Login == login || acc.Password == password)
                                {
                                    new Editing().EditStudent((int)studId, name.TrimEnd().TrimStart(), surname.TrimEnd().TrimStart(), login, password, groupId);
                                }
                            }
                        }
                        intbook.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                        Inte.Quit();
                        foreach (var student in students)
                        {
                            if (!ids.Contains(student.Id))
                            {
                                new Deleting().DeleteStudent(student.Id);
                            }
                        }
                        TempData["Success"] = "Зміни по студентах групи - \""+new testingDbEntities().Groups.FirstOrDefault(t => t.Id == groupId).Name+"\" було успішно збережено!";
                    }
                    catch (Exception e)
                    {
                        var error = e;
                        intbook.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                        Inte.Quit();
                    }
                }
            }
            return RedirectToAction("Students");
        }
        public ActionResult EditStudent(UserViewModel model)
        {
            try
            {
                new Editing().EditStudent(model.Id, model.Name.TrimEnd().TrimStart(), model.Surname.TrimEnd().TrimStart(), model.Login, model.Password, model.GroupId);
                TempData["Success"] = "Зміни було успішно збережено!";
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Students");
        }
        public ActionResult DeleteStudent(int studentId)
        {
            try
            {
                new Deleting().DeleteStudent(studentId);
                TempData["Success"] = "Студент був успішно видалений!";
            }
            catch (Exception)
            {
            }
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
            try
            {
                new Adding().AddNewLector(model.Name.TrimEnd().TrimStart(), model.Surname.TrimEnd().TrimStart());
                TempData["Success"] = "Лектор - "+ model.Name.TrimEnd().TrimStart()+" "+ model.Surname.TrimEnd().TrimStart() + " успішно доданий!";
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Lectors");
        }
        public ActionResult EditLector(UserViewModel model)
        {
            try
            {
                new Editing().EditLector(model.Id, model.Name.TrimEnd().TrimStart(), model.Surname.TrimEnd().TrimStart(), model.Login, model.Password);
                TempData["Success"] = "Зміни було успішно збережено!";
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Lectors");
        }
        public ActionResult DeleteLector(int lectorId)
        {
            try
            {
                new Deleting().DeleteLector(lectorId);
                TempData["Success"] = "Лектор був успішно видалений!";
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Lectors");
        }

        //DisciplineStudents
        public ActionResult DisciplineStudents(int disciplineId)
        {
            var db = new testingDbEntities();
            IEnumerable<Group> grp = db.Groups.ToList();
            IEnumerable<Speciality> spc = db.Specialities.ToList();
            IEnumerable<Student> std = db.Students.OrderBy(t => t.GroupId).ToList();
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
            try
            {
                if (model.StudentDisciplines != null)
                {
                    new Adding().AddNewStudentConnection(model);
                    TempData["Success"] = "Зміни було успішно збережено!";
                }
            }
            catch (Exception)
            {
            }
            return RedirectToAction("DisciplineStudents");
        }
    }
}