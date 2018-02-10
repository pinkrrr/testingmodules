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

    public class StudentPageHelper
    {
        private readonly testingDbEntities _db = new testingDbEntities();
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
                if (await (from mh in _db.ModuleHistories
                           join lh in _db.LecturesHistories on mh.LectureHistoryId equals lh.Id
                           join lhg in _db.LectureHistoryGroups on mh.LectureHistoryId equals lhg.Id
                           join s in _db.Students on lhg.GroupId equals s.GroupId
                           where s.Id == studentId
                           join sd in _db.StudentDisciplines on s.Id equals sd.StudentId
                           where sd.DisciplineId == lh.DisciplineId
                           select s).AnyAsync())
                    return true;
            }
            return false;
        }
    }
}