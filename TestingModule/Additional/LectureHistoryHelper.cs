using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{
    public class LectureHistoryHelper
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public async Task<int> StartLecture(ReasignViewModel model)
        {
            var disc = model.Disciplines[0].Id;
            var lect = model.Lectures[0].Id;
            var date = DateTime.UtcNow;
            var lector = await new AccountCredentials().GetLector();
            _db.LecturesHistories.Add(new LecturesHistory { LectureId = lect, DisciplineId = disc, StartTime = date, ModulesPassed = 0, LectorId = lector.Id });
            await _db.SaveChangesAsync();

            int lectureHistoryId = _db.LecturesHistories.Local.Where(lh => lh.StartTime == date).Select(lh => lh.Id)
                .SingleOrDefault();

            _db.ModuleHistories.AddRange(
                from m in await _db.Modules.ToListAsync()
                where m.LectureId == lect
                select new ModuleHistory
                {
                    IsPassed = false,
                    LectureHistoryId = lectureHistoryId,
                    ModuleId = m.Id,
                    StartTime = null
                });

            _db.LectureHistoryGroups.AddRange(
                from g in model.Groups
                where g.IsSelected
                select new LectureHistoryGroup
                {
                    GroupId = g.Id,
                    LectureHistoryId = lectureHistoryId
                });

            await _db.SaveChangesAsync();
            return lectureHistoryId;
        }

        public async Task<ActiveLectureViewModel> GetActiveLecture(int lectureHistoryId)
        {
            Lector lector = await new AccountCredentials().GetLector();
            LecturesHistory lecturesHistory = await _db.LecturesHistories.
                SingleOrDefaultAsync(lh => lh.Id == lectureHistoryId);
            Lecture lecture = await _db.Lectures.SingleOrDefaultAsync(l => l.Id == lecturesHistory.LectureId);
            Discipline discipline = await _db.Disciplines.SingleOrDefaultAsync(d => d.Id == lecture.DisciplineId);
            IEnumerable<Module> modules = await _db.Modules.Where(m => m.LectureId == lecture.Id).ToListAsync();
            IEnumerable<ModuleHistory> moduleHistories = await _db.ModuleHistories
                .Where(mh => mh.LectureHistoryId == lectureHistoryId).ToListAsync();
            return new ActiveLectureViewModel
            {
                Lector = lector,
                Discipline = discipline,
                Lecture = lecture,
                LecturesHistory = lecturesHistory,
                ModuleHistories = moduleHistories,
                Modules = modules
            };
        }

        public void StopLecture(string login)
        {
            var lector = _db.Accounts.FirstOrDefault(t => t.Login == login).Id;
            var lectorId = _db.Lectors.FirstOrDefault(t => t.AccountId == lector).Id;
            var lectorsDisciplines = _db.LectorDisciplines.Where(t => t.LectorId == lectorId).Select(t => t.DisciplineId)
                .ToList();
            var activeLectures = _db.LecturesHistories
                .Where(t => lectorsDisciplines.Contains(t.DisciplineId) && t.EndTime == null).ToList();
            var activeLecId = _db.LecturesHistories
                .Where(t => lectorsDisciplines.Contains(t.DisciplineId) && t.EndTime == null).Select(t => t.Id).ToList();
            _db.ModuleHistories.RemoveRange(_db.ModuleHistories.Where(t => activeLecId.Contains(t.LectureHistoryId)));
            foreach (var lec in activeLectures)
            {
                lec.EndTime = DateTime.Now;
                _db.SaveChanges();
            }

        }

        public async Task StartModule(int moduleHistoryId)
        {
            ModuleHistory moduleHistory =
                await _db.ModuleHistories.SingleOrDefaultAsync(mh => mh.Id == moduleHistoryId);
            if (moduleHistory.IsPassed)
            {
                var responsesToDelete =
                    from r in _db.Respons
                    where r.LectureHistoryId == moduleHistory.LectureHistoryId
                    join q in _db.Questions on r.QuestionId equals q.Id
                    where q.ModuleId == moduleHistory.ModuleId
                    select r;
                _db.Respons.RemoveRange(responsesToDelete);
                moduleHistory.IsPassed = false;
            }
               
            moduleHistory.StartTime=DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task<int> ModulePassed(int moduleHistoryId)
        {
            ModuleHistory moduleHistory =
                await _db.ModuleHistories.SingleOrDefaultAsync(mh => mh.Id == moduleHistoryId);
            moduleHistory.IsPassed = true;
            await _db.SaveChangesAsync();
            return moduleHistory.LectureHistoryId;
        }

    }
}