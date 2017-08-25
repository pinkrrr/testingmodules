using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index()
        {
            var db = new testingDbEntities();
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
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var c = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var login = c.Value.ToString();

            var account = db.Accounts.FirstOrDefault(t => t.Login == login).Id;
            var student = db.Students.FirstOrDefault(t => t.AccountId == account).Id;
            var connect = db.StudentDisciplines.Where(t => t.StudentId == student).Select(t => t.DisciplineId).ToList();
            var group = db.Students.FirstOrDefault(t => t.Id == student).GroupId;
            var active = db.LecturesHistories.Where(t => connect.Contains(t.DisciplineId) && t.EndTime == null).Select(t => t.Id)
                .ToList();
            if (active.Any())
            {
                var groupsToDisc =
                    db.LectureHistoryGroups.Where(t => active.Contains(t.LectureHistoryId) && t.GroupId == group);
                if (groupsToDisc.Any())
                {
                    var activeModule = db.ModuleHistories.FirstOrDefault(t => active.Contains(t.LectureHistoryId))
                        .ModuleId;
                    if (activeModule != null)
                    {
                        return RedirectToAction("/"+activeModule, "Quiz");
                    }
                }
            }
           
            viewModels = viewModels.Where(t => connect.Contains(t.Id)).ToList();
            return View(viewModels);
        }
    }
}