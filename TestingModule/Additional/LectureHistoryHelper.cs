using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Win32.SafeHandles;
using TestingModule.Controllers;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{
    public class LectureHistoryHelper
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        private readonly TimerAssociates _timerAssociates=new TimerAssociates();

        public async Task StartLecture(ReasignViewModel model)
        {
            var disc = model.Disciplines[0].Id;
            var lect = model.Lectures[0].Id;
            var date = DateTime.UtcNow;
            var lector = await new AccountCredentials().GetLector();
            _db.LecturesHistories.Add(new LecturesHistory
            {
                LectureId = lect,
                DisciplineId = disc,
                StartTime = date,
                IsFrozen = false,
                LectorId = lector.Id
            });
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
                    StartTime = null,
                    LectorId = lector.Id
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
        }

        public async Task<ActiveLectureViewModel> GetActiveLecture(Lector lector)
        {
            LecturesHistory lecturesHistory =
                 await _db.LecturesHistories.SingleOrDefaultAsync(lh => lh.IsFrozen == false && lh.EndTime == null && lh.LectorId == lector.Id);
            Lecture lecture = await _db.Lectures.SingleOrDefaultAsync(l => l.Id == lecturesHistory.LectureId);
            Discipline discipline = await _db.Disciplines.SingleOrDefaultAsync(d => d.Id == lecture.DisciplineId);
            IEnumerable<Module> modules = await _db.Modules.Where(m => m.LectureId == lecture.Id).ToListAsync();
            IEnumerable<ModuleHistory> moduleHistories = await _db.ModuleHistories
                .Where(mh => mh.LectureHistoryId == lecturesHistory.Id).ToListAsync();
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

        public async Task StopLecture(int lectureHistoryId)
        {
            var lecturesHistory = await _db.LecturesHistories.SingleOrDefaultAsync(lh => lh.Id == lectureHistoryId);
            lecturesHistory.EndTime = DateTime.UtcNow;
            _db.ModuleHistories.RemoveRange(_db.ModuleHistories.Where(t => t.LectureHistoryId == lectureHistoryId));
            await _db.SaveChangesAsync();
        }

        public async Task SetLectureAsFrozen(int lectureHistoryId)
        {
            LecturesHistory toFreeze = await _db.LecturesHistories.SingleOrDefaultAsync(lh => lh.Id == lectureHistoryId);
            toFreeze.IsFrozen = true;
            await _db.SaveChangesAsync();
        }

        public async Task UnfreezeLecture(int lectureHistoryId)
        {
            LecturesHistory toUnfreeze = await _db.LecturesHistories.SingleOrDefaultAsync(lh => lh.Id == lectureHistoryId);
            toUnfreeze.IsFrozen = false;
            await _db.SaveChangesAsync();
        }

        public async Task StartModule(int moduleHistoryId)
        {
            ModuleHistory moduleHistory =
                await _db.ModuleHistories.SingleOrDefaultAsync(mh => mh.Id == moduleHistoryId);
            if (moduleHistory.IsPassed)
            {
                var responsesToDelete =
                    from r in _db.RealtimeResponses
                    where r.LectureHistoryId == moduleHistory.LectureHistoryId
                    join q in _db.Questions on r.QuestionId equals q.Id
                    where q.ModuleId == moduleHistory.ModuleId
                    select r;
                _db.RealtimeResponses.RemoveRange(responsesToDelete);
                moduleHistory.IsPassed = false;
            }
            moduleHistory.StartTime = DateTime.UtcNow;
            TimeSpan minutesToPass = TimeSpan.FromMinutes(await _db.Modules.Where(m => m.Id == moduleHistory.ModuleId)
                .Select(m => m.MinutesToPass).SingleOrDefaultAsync());
            _timerAssociates.StartTimer(moduleHistoryId, minutesToPass,TimerAssociates.TimerType.RealtimeId);
            await _db.SaveChangesAsync();
        }
        
        public async Task ModulePassed(int moduleHistoryId)
        {
            ModuleHistory moduleHistory =
                await _db.ModuleHistories.SingleOrDefaultAsync(mh => mh.Id == moduleHistoryId);
            moduleHistory.IsPassed = true;
            await _db.SaveChangesAsync();
            _timerAssociates.DisposeTimer(moduleHistoryId,TimerAssociates.TimerType.RealtimeId);
        }
    }
}