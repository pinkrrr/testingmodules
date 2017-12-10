﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
                DiscId = d.Id,
                DiscName = d.Name
            }
            ).ToList();
            foreach (var model in viewModels)
            {
                model.Lectors = lectors;
                model.LectorId = _db.LectorDisciplines.Where(t => model.DiscId == t.DisciplineId).Select(t => t.LectorId)
                    .FirstOrDefault();
            }
            var c = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var login = c.Value.ToString();

            var account = _db.Accounts.FirstOrDefault(t => t.Login == login).Id;
            var student = _db.Students.FirstOrDefault(t => t.AccountId == account).Id;
            var connect = _db.StudentDisciplines.Where(t => t.StudentId == student).Select(t => t.DisciplineId).ToList();
            viewModels = viewModels.Where(t => connect.Contains(t.DiscId)).ToList();
            return (viewModels);
        }

        public async Task<List<int>> CheckActiveQuiz(int moduleHistoryId, int studentId)
        {
            if (!await _db.ModuleHistories.AnyAsync(mh =>
                mh.Id == moduleHistoryId && mh.StartTime != null && mh.IsPassed != true))
                return null;
            if (
                !await (
                    from mh in _db.ModuleHistories
                    join lh in _db.LecturesHistories on mh.LectureHistoryId equals lh.Id
                    join lhg in _db.LectureHistoryGroups on mh.LectureHistoryId equals lhg.Id
                    join s in _db.Students on lhg.GroupId equals s.GroupId
                    where s.Id == studentId
                    join sd in _db.StudentDisciplines on s.Id equals sd.StudentId
                    where sd.DisciplineId == lh.DisciplineId
                    select s).AnyAsync())
                return null;
            var answeredQuestions = await _db.Respons.Where(t => t.StudentId == studentId &&
                                                                 t.ModuleHistoryId == moduleHistoryId).Select(t => t.QuestionId).ToListAsync();
            return answeredQuestions;

        }
    }
}