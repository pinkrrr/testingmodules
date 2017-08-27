using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using TestingModule.Controllers;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{
    public class StudentPageHelper : StudentController
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public List<DiscLecotorViewModel> StudentsDisciplinesList(ClaimsIdentity claimsIdentity)
        {
            IEnumerable<Lector> lectors = _db.Lectors.ToList();
            List<DiscLecotorViewModel> viewModels = _db.Disciplines.Select(d => new DiscLecotorViewModel
            {
                Id = d.Id,
                Name = d.Name
            }
            ).ToList();
            foreach (var model in viewModels)
            {
                model.Lectors = lectors;
                model.LectorId = _db.LectorDisciplines.Where(t => model.Id == t.DisciplineId).Select(t => t.LectorId)
                    .FirstOrDefault();
            }
            var c = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var login = c.Value.ToString();

            var account = _db.Accounts.FirstOrDefault(t => t.Login == login).Id;
            var student = _db.Students.FirstOrDefault(t => t.AccountId == account).Id;
            var connect = _db.StudentDisciplines.Where(t => t.StudentId == student).Select(t => t.DisciplineId).ToList();
            viewModels = viewModels.Where(t => connect.Contains(t.Id)).ToList();
            return (viewModels);
        }

        public int? CheckActiveQuiz(ClaimsIdentity claimsIdentity)
        {
            var c = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var login = c.Value.ToString();
            var account = _db.Accounts.FirstOrDefault(t => t.Login == login).Id;
            var student = _db.Students.FirstOrDefault(t => t.AccountId == account).Id;
            var connect = _db.StudentDisciplines.Where(t => t.StudentId == student).Select(t => t.DisciplineId).ToList();
            var group = _db.Students.FirstOrDefault(t => t.Id == student).GroupId;
            var active = _db.LecturesHistories.Where(t => connect.Contains(t.DisciplineId) && t.EndTime == null).Select(t => t.Id)
                .ToList();
            if (active.Any())
            {
                var groupsToDisc =
                    _db.LectureHistoryGroups.Where(t => active.Contains(t.LectureHistoryId) && t.GroupId == group);
                if (groupsToDisc.Any())
                {
                    var activeModule = _db.ModuleHistories.FirstOrDefault(t => active.Contains(t.LectureHistoryId));
                    if (activeModule != null)
                    {
                        var moduleQuestions = _db.Questions.Where(t => t.ModuleId == activeModule.ModuleId).Select(t => t.Id)
                            .ToList();
                        var studentResponses =
                            _db.Respons.Where(t => t.StudentId == student &&
                                                     t.LectureHistoryId == activeModule.LectureHistoryId &&
                                                     moduleQuestions.Contains(t.QuestionId)).ToList();
                        if (studentResponses.Count != moduleQuestions.Count)
                        {
                            return activeModule.ModuleId;
                        }

                    }
                }
            }
            return null;
        }
    }
}