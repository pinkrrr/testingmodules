using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.DynamicData;
using Microsoft.Owin.Security.Provider;
using TestingModule.Controllers;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{

    public class StudentPageHelper : IDisposable
    {
        private readonly testingDbEntities _db;

        public StudentPageHelper(testingDbEntities context)
        {
            _db = context;
        }

        public async Task<IEnumerable<DiscLectorCumulativeCheckViewModel>> StudentsDisciplinesList()
        {
            var studentId = AccountCredentials.GetStudentId();
            List<DiscLectorCumulativeCheckViewModel> model =
                await (from d in _db.Disciplines
                       join sd in _db.StudentDisciplines on d.Id equals sd.DisciplineId
                       where sd.StudentId == studentId
                       join cq in _db.CumulativeQuizPasseds on sd.DisciplineId equals cq.DisciplineId into cqsjoin
                       from cqs in cqsjoin.DefaultIfEmpty()
                       where cqs.StudentId == studentId || cqs == null
                       join ld in _db.LectorDisciplines on d.Id equals ld.DisciplineId
                       join l in _db.Lectors on ld.LectorId equals l.Id into lectjoin
                       select new DiscLectorCumulativeCheckViewModel()
                       {
                           DiscId = d.Id,
                           DiscName = d.Name,
                           Lectors = lectjoin,
                           CumulativeQuizId = cqs.IsPassed != true && cqs != null ? cqs.Id : (int?)null
                       }).ToListAsync();
            return model;
        }

        public async Task<bool> StudentCanPass(int moduleHistoryId, int studentId)
        {
            if (await _db.ModuleHistories.AnyAsync(mh =>
                mh.Id == moduleHistoryId && mh.StartTime != null && mh.IsPassed != true))
            {
                var canPass = await (from lh in _db.LecturesHistories
                                     join mh in _db.ModuleHistories on lh.Id equals mh.LectureHistoryId
                                     where mh.Id == moduleHistoryId
                                     join lhg in _db.LectureHistoryGroups on lh.Id equals lhg.LectureHistoryId
                                     join s in _db.Students on lhg.GroupId equals s.GroupId
                                     where s.Id == studentId
                                     join sd in _db.StudentDisciplines on lh.DisciplineId equals sd.DisciplineId
                                     select sd).AnyAsync();
                return canPass;
            }
            return false;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}