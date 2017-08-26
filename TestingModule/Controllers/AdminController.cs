using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TestingModule.Additional;
using TestingModule.Models;
using TestingModule.ViewModels;
using Module = TestingModule.Models.Module;

namespace TestingModule.Controllers
{
    [CustomAuthorize(RoleName.Administrator, RoleName.Lecturer)]
    public class adminController : Controller
    {
        public ActionResult Index()
        {
            var db = new testingDbEntities();
            ReasignViewModel model = new ReasignViewModel();
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var login = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value.ToString();
            var role = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Role).Value.ToString();
            
            if (role == "Lecturer")
            {
                var lector = db.Accounts.FirstOrDefault(t => t.Login == login).Id;
                var lectorId = db.Lectors.FirstOrDefault(t => t.AccountId == lector).Id;
                var lectorsDisciplines = db.LectorDisciplines.Where(t => t.LectorId == lectorId).Select(t => t.DisciplineId)
                    .ToList();
                var students = db.StudentDisciplines.Where(t => lectorsDisciplines.Contains(t.DisciplineId))
                    .Select(t => t.StudentId).ToList();
                var groups = db.Students.Where(t => students.Contains(t.Id)).Select(t => t.GroupId).ToList();
                model.Disciplines = db.Disciplines.Where(t => lectorsDisciplines.Contains(t.Id)).ToList();
                model.Modules = db.Modules.Where(t => lectorsDisciplines.Contains(t.DisciplineId)).ToList();
                model.Lectures = db.Lectures.Where(t => lectorsDisciplines.Contains(t.DisciplineId)).ToList();
                model.Groups = db.Groups.Where(t => groups.Contains(t.Id)).ToList();
                model.LecturesHistories = db.LecturesHistories.Where(t => t.StartTime != null && t.EndTime == null)
                    .ToList();
                model.ModuleHistories = db.ModuleHistories.ToList();
                var startedLectures = db.LecturesHistories
                    .Where(t => lectorsDisciplines.Contains(t.Id) && t.EndTime == null).ToList();
                if (startedLectures.Any())
                {
                    model.LecturesHistories = startedLectures;
                }
                return View(model);
            }

            return View();
        }
        //LectureHistory
        public ActionResult StartLecture(ReasignViewModel model)
        {
            if (model.Disciplines != null && model.Lectures != null && model.Groups != null)
            {
                new LectureHistoryHelper().StartLecture(model);
            }
            else
            {
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
        public ActionResult StopLecture()
        {
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var login = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value.ToString();
            new LectureHistoryHelper().StopLecture(login);
            return RedirectToAction("Index");
        }
        public ActionResult StartModule(int moduleId)
        {
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var login = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value.ToString();
            new LectureHistoryHelper().StartModule(moduleId,login);
            return RedirectToAction("Index");
        }
        public ActionResult StopModule(int moduleId)
        {
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var login = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value.ToString();
            new LectureHistoryHelper().StopModule(moduleId, login);
            return RedirectToAction("Index");
        }

        //Discipline
        public ActionResult Disciplines()
        {
            var db = new testingDbEntities();
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var login = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value.ToString();
            var role = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Role).Value.ToString();
            var lector = 0;
            var lectorId = 0;
            var lectorsDisciplines = new List<int>();

            ViewBag.Message = "All disciplines";
            IEnumerable<Lector> lectors = db.Lectors.ToList();
            List<DiscLecotorViewModel> viewModels = new List<DiscLecotorViewModel>();
            if (role == "Lecturer")
            {
                lector = db.Accounts.FirstOrDefault(t => t.Login == login).Id;
                lectorId = db.Lectors.FirstOrDefault(t => t.AccountId == lector).Id;
                lectorsDisciplines = db.LectorDisciplines.Where(t => t.LectorId == lectorId).Select(t => t.DisciplineId)
                   .ToList();
                viewModels = db.Disciplines.Where(t => lectorsDisciplines.Contains(t.Id)).Select(d => new DiscLecotorViewModel
                {
                    Id = d.Id,
                    Name = d.Name
                }
                ).ToList();
            }
            else
            {
                viewModels = db.Disciplines.Select(d => new DiscLecotorViewModel
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "NewDiscipline Name = " + model.Name + " LectorId = " + model.LectorId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
            }

            return RedirectToAction("Disciplines");
        }
        public ActionResult EditDiscipline(DiscLecotorViewModel model)
        {
            try
            {
                new Editing().EditDiscipline(model.Id, model.Name.TrimEnd().TrimStart(), model.LectorId);
                TempData["Success"] = "Зміни було успішно збережено";
            }
            catch (Exception)
            {
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "EditDiscipline Name = " + model.Name + " LectorId = " + model.LectorId + " Id = " + model.Id);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "DeleteDiscipline ID = " + disciplineId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
            }
            return RedirectToAction("Disciplines");
        }


        //Lecture
        public ActionResult Lectures(int disciplineId)
        {
            IList<Lecture> lect = new testingDbEntities().Lectures.Where(t => t.DisciplineId == disciplineId).ToList();
            IList<Discipline> disc = new testingDbEntities().Disciplines.ToList();
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "NewLecture Name = " + model.Name + " DisciplineId = " + model.DisciplineId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "EditLecture Name = " + model.Name + " DiciplineId = " + model.DisciplineId + " LectureId = " + model.Id);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "DeleteLecture Id = " + lectureId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
            }
            return RedirectToAction("Lectures");
        }



        //Module
        public ActionResult Modules(int lectureId)
        {
            var db = new testingDbEntities();
            var discId = db.Lectures.FirstOrDefault(t => t.Id == lectureId).DisciplineId;
            IList<Module> mod = db.Modules.Where(t => t.LectureId == lectureId).ToList();
            IList<Lecture> lect = db.Lectures.Where(t => t.DisciplineId == discId).ToList();
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "NewModule Name = " + model.Name + " LectureId = " + model.LectureId
                    + " DisciplineId = " + model.DisciplineId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
            }
            return RedirectToAction("Modules");
        }
        public ActionResult EditModule(Module model)
        {
            try
            {
                if (model.Id != null && model.Name != null)
                {
                    new Editing().EditModule(model.Id, model.Name.TrimEnd().TrimStart(), model.LectureId);
                    TempData["Success"] = "Зміни було успіщно збережено!";
                }

            }
            catch (Exception)
            {
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "EditModule Name = " + model.Name + " LectureId = " + model.LectureId + " Id = " + model.Id);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "DeleteModule Id = " + moduleId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "NewQuestion Name = " + model.Question + " LectureId = "
                    + model.LectureId + " DiscpilineId = " + model.DisciplineId + " ModuleId = " + model.ModuleId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "EditQuestion Name = " + model.Question + " ModuleId = "
                    + model.ModuleId + " Id = " + model.QuestionId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "DeleteQuestion Id = " + questionId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "NewAnswer Name = " + model.Answer + " QuestionId = " + model.QuestionId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
            }
            return RedirectToAction("Questions");
        }
        public ActionResult EditAnswer(List<QueAns> model)
        {
            if (model != null)
            {
                try
                {
                    var modelIndex = 0;
                    foreach (var item in model.Where(t => t.ModuleId != null && t.ModuleId != 0))
                    {
                        modelIndex = model.IndexOf(item);
                        break;
                    }
                    foreach (var item in model.Where(t => t.Answer != null))
                    {
                        new Editing().EditAnswer(item.AnswerId, item.Answer.TrimEnd().TrimStart(), item.IsCorrect);
                    }
                    var firstOrDefault = model[modelIndex];
                    if (firstOrDefault != null)
                        new Editing().EditQuestion(firstOrDefault.QuestionId,
                            firstOrDefault.Question.TrimEnd().TrimStart()
                            , firstOrDefault.ModuleId);
                    TempData["Success"] = "Зміни по запитаннях та відповідях було успішно збережено!";
                }
                catch (Exception)
                {
                    HttpContext con = System.Web.HttpContext.Current;
                    var url = con.Request.Url.ToString();
                    new Adding().AddNewError(url, "EditAnswer Answers = " + model.FirstOrDefault().Answer + " AnswerId = "
                                                  + model.FirstOrDefault().AnswerId + " Question = " + model.FirstOrDefault().Question + " QuestionId = " + model.FirstOrDefault().QuestionId);
                    TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
                }
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "DeleteAnswer Id = " + answerId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "NewSpeciality Name = " + model.Name);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "EditSpeciality Name = " + model.Name + " Id = " + model.Id);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "DeleteSpeciality Id = " + specialityId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
            }
            return RedirectToAction("Specialities");
        }



        //Groups
        public ActionResult Groups(int specialityId)
        {
            var db = new testingDbEntities();
            IList<Group> grp = db.Groups.Where(t => t.SpecialityId == specialityId).ToList();
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "NewGroup Name = " + model.Name + " SpecialityId = " + model.SpecialityId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
            catch (Exception)
            {
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "EditGroup Name = " + model.Name + " GroupId = " + model.Id + " SpecialityId = " + model.SpecialityId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "DeleteGroup Id = " + groupId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
            }
            return RedirectToAction("Groups");
        }

        //Students
        public ActionResult Students(int GroupId)
        {
            var db = new testingDbEntities();
            var specId = db.Groups.FirstOrDefault(t => t.Id == GroupId).SpecialityId;
            List<int> accList = new List<int>();
            IEnumerable<Student> std = db.Students.Where(t => t.GroupId == GroupId).ToList();
            foreach (var student in std)
            {
                accList.Add(student.AccountId);
            }
            IEnumerable<Account> acc = db.Accounts.Where(t => accList.Contains(t.Id)).ToList();
            IList<Group> grp = db.Groups.Where(t => t.SpecialityId == specId).ToList();
            ReasignViewModel test = new ReasignViewModel() { Groups = grp, Accounts = acc, Students = std };
            return View(test);
        }
        public ActionResult NewStudent(Student model)
        {
            try
            {
                new Adding().AddNewStudent(model.Name.TrimEnd().TrimStart(), model.Surname.TrimEnd().TrimStart(), model.GroupId, model.SpecialityId);
                TempData["Success"] = "Студент - \"" + model.Name.TrimEnd().TrimStart() + " " + model.Surname.TrimEnd().TrimStart() + "\" був успішно доданий!";
            }
            catch (Exception)
            {
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "NewStudent Name = " + model.Name + " Surname = " + model.Surname + " SpecialityId = "
                    + model.SpecialityId + " GroupId = " + model.GroupId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
            }
            return RedirectToAction("Students");
        }

        public ActionResult DownloadStudentExcel(int GroupId)
        {
            var db = new testingDbEntities();
            var group = db.Groups.FirstOrDefault(t => t.Id == GroupId).Name;
            var students = db.Students.Where(t => t.GroupId == GroupId).ToList();
            var account = db.Accounts.ToList();

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(group);
                var filter = "";
                if (students.Count != 0)
                {
                    using (ExcelRange rng = ws.Cells["G1:G4"])
                    {
                        rng.Style.Font.Size = 12;
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rng.Style.Fill.BackgroundColor.SetColor(Color.Black);
                        rng.Style.Font.Color.SetColor(Color.White);
                        rng.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                        rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    ws.Cells[2, 3].Value = "StudentId";
                    ws.Cells[2, 4].Value = "Обліковий запис";
                    ws.Cells[2, 5].Value = "Пароль";
                    ws.Cells[2, 7].Value =
                        "2 - для того, щоб редагувати ім'я, прізвище, обліковий запис, або пароль студента, змініть необхідні дані на його рядку в колонках рядка в колонка A, B, D, або E";
                    ws.Cells[3, 7].Value =
                        "3 - для того, щоб видалити студента, видаліть рядок студента повністю за допомогою виділення рядка, та його видалення. Важливо, щоб між рядками студентів не було порожних рядків!";
                    ws.Cells[4, 7].Value =
                        "4 - після заврешення редагування документу, збережіть його і завантажте на сайті в необхідну групу.";
                    filter = "E";
                }
                else
                {
                    using (ExcelRange rng = ws.Cells["G1:G2"])
                    {
                        rng.Style.Font.Size = 12;
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rng.Style.Fill.BackgroundColor.SetColor(Color.Black);
                        rng.Style.Font.Color.SetColor(Color.White);
                        rng.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                        rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    ws.Cells[2, 7].Value =
                        "2 - після заврешення редагування документу, збережіть його і завантажте на сайті в необхідну групу.";
                    filter = "B";
                }
                using (ExcelRange rng = ws.Cells["A1:" + filter + "2"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Black);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                ws.Cells[1, 1].Value = group;
                ws.Cells[1, 1].Style.Font.Size = 14;
                ws.Cells[2, 1].Value = "Прізвище";
                ws.Cells[2, 2].Value = "Ім'я";
                ws.Cells[1, 7].Value =
                    "1 - для того, щоб додати нового студента, введіть його ім'я та прізвище з нового рядка в колонка А та В";

                foreach (var stud in students)
                {
                    var row = 3 + students.IndexOf(stud);
                    ws.Cells[row, 1].Value = stud.Surname;
                    ws.Cells[row, 2].Value = stud.Name;
                    ws.Cells[row, 3].Value = stud.Id;
                    ws.Cells[row, 4].Value = account.FirstOrDefault(t => t.Id == stud.AccountId).Login;
                    ws.Cells[row, 5].Value = account.FirstOrDefault(t => t.Id == stud.AccountId).Password;
                }
                ws.Cells["A1:" + filter + "1"].Merge = true;
                ws.Column(7).AutoFit();
                ws.Cells.AutoFitColumns();
                //Write it back to the client
                using (var memoryStream = new MemoryStream())
                {
                    var fileName = group;
                    Encoding encoding = Encoding.UTF8;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.Charset = encoding.EncodingName;
                    Response.ContentEncoding = Encoding.Unicode;
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + HttpUtility.UrlEncode(fileName, Encoding.UTF8) + ".xlsx\"");
                    pck.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                pck.Dispose();
            }
            return null;
        }

        [HttpPost]
        public ActionResult UploadStudentExcel(int groupId)
        {
            if (Request.Files.Count != 0)
            {
                string ext = Path.GetExtension(Request.Files[0].FileName);
                var validExtensions = new[] { ".xlsx", ".xls", "csv" };
                if (!validExtensions.Contains(ext))
                {
                    TempData["FailUpload"] =
                        "Невірний формат файлу! Тільки файли створенні за допомогою Excel підтримуються (.xlsx ; .xls)";
                    return RedirectToAction("Students");
                }
                var file = Request.Files[0];
                MemoryStream mem = new MemoryStream();
                mem.SetLength((int)file.ContentLength);
                file.InputStream.Read(mem.GetBuffer(), 0, (int)file.ContentLength);
                try
                {
                    using (ExcelPackage p = new ExcelPackage(mem))
                    {
                        {
                            ExcelWorksheet ws = p.Workbook.Worksheets[1];
                            var specialityId = new testingDbEntities().Groups.FirstOrDefault(t => t.Id == groupId).SpecialityId;
                            var students = new testingDbEntities().Students.Where(t => t.GroupId == groupId && t.SpecialityId == specialityId).ToList();
                            var accounts = new testingDbEntities().Accounts.ToList();
                            List<int> ids = new ListStack<int>();
                            for (int i = 0; i < 100; i++)
                            {
                                if (ws.Cells[3 + i, 1].Value == null)
                                {
                                    break;
                                }
                                var name = ws.Cells[3 + i, 2].Value.ToString();
                                var surname = ws.Cells[3 + i, 1].Value.ToString();
                                var studId = Convert.ToInt32(ws.Cells[3 + i, 3].Value);
                                var login = Convert.ToString(ws.Cells[3 + i, 4].Value);
                                var password = Convert.ToString(ws.Cells[3 + i, 5].Value);
                                ids.Add(studId);
                                if (!students.Any(t => t.Id == studId) && studId == 0)
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
                            foreach (var student in students)
                            {
                                if (!ids.Contains(student.Id))
                                {
                                    new Deleting().DeleteStudent(student.Id);
                                }
                            }
                            TempData["Success"] = "Зміни по студентах групи - \"" + new testingDbEntities().Groups.FirstOrDefault(t => t.Id == groupId).Name + "\" було успішно збережено!";
                            p.Dispose();
                        }

                    }
                }
                catch (Exception)
                {
                    TempData["FailUpload"] =
                        "Невірне оформлення документу! Будь ласка, завантажте шаблон і заповніть згідно вказаних праввил.";
                }

            }
            return RedirectToAction("Students");
        }
        public ActionResult EditStudent(UserViewModel model)
        {
            try
            {
                if (model.Name != model.Surname && model.Name != model.Login)
                {
                    new Editing().EditStudent(model.Id, model.Name.TrimEnd().TrimStart(), model.Surname.TrimEnd().TrimStart(), model.Login, model.Password, model.GroupId);
                    TempData["Success"] = "Зміни було успішно збережено!";
                }
            }
            catch (Exception)
            {
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "EditStudent Name = " + model.Name + " Surname = " + model.Surname
                    + " Login = " + model.Login + " Password = " + model.Password + " GroupId = " + model.GroupId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "DeleteStudent ID = " + studentId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
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
                TempData["Success"] = "Лектор - " + model.Name.TrimEnd().TrimStart() + " " + model.Surname.TrimEnd().TrimStart() + " успішно доданий!";
            }
            catch (Exception)
            {
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "NewLector Name = " + model.Name + " Surname = " + model.Surname);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
            }
            return RedirectToAction("Lectors");
        }
        public ActionResult EditLector(UserViewModel model)
        {
            if (model.Surname != null && model.Login != null && model.Password != null)
            {
                if (model.Name != model.Surname && model.Name != model.Login)
                {
                    try
                    {
                        new Editing().EditLector(model.Id, model.Name.TrimEnd().TrimStart(),
                            model.Surname.TrimEnd().TrimStart(), model.Login, model.Password);
                        TempData["Success"] = "Зміни було успішно збережено!";
                    }
                    catch (Exception)
                    {
                        HttpContext con = System.Web.HttpContext.Current;
                        var url = con.Request.Url.ToString();
                        new Adding().AddNewError(url, "EditLector Name = " + model.Name + " Surname = "
                                                      + model.Surname + " Id = " + model.Id + " Login = " +
                                                      model.Login + " Password = " + model.Password);
                        TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
                    }
                }
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "DeleteLector Id = " + lectorId);
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
            }
            return RedirectToAction("Lectors");
        }

        //DisciplineStudents
        public ActionResult DisciplineStudents(int disciplineId)
        {
            var db = new testingDbEntities();
            IList<Group> grp = db.Groups.ToList();
            IEnumerable<Speciality> spc = db.Specialities.ToList();
            IEnumerable<Student> std = db.Students.OrderBy(t => t.GroupId).ThenBy(n => n.Surname).ToList();
            IList<StudentDiscipline> studDisc = db.StudentDisciplines.Where(t => t.DisciplineId == disciplineId).ToList();
            IList<Discipline> disc = db.Disciplines.Where(t => t.Id == disciplineId).ToList();
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
                HttpContext con = System.Web.HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Adding().AddNewError(url, "NewStudentConnetctions");
                TempData["Fail"] = "Щось пішло не так. Перевірте правильність дій";
            }
            return RedirectToAction("DisciplineStudents");
        }
    }
}