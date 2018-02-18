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
using TestingModule.Hubs;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{
    public class LectureHistoryHelper : IDisposable
    {
        private readonly testingDbEntities _db;

        public LectureHistoryHelper()
        {
            _db = new testingDbEntities();
        }

        public async Task StartLecture(ReasignViewModel model)
        {
            var disc = model.Disciplines[0].Id;
            var lect = model.Lectures[0].Id;
            var date = DateTime.UtcNow;
            var lector = await AccountCredentials.GetLector();
            var lectureHistory = _db.LecturesHistories.Add(new LecturesHistory
            {
                LectureId = lect,
                DisciplineId = disc,
                StartTime = date,
                IsFrozen = false,
                LectorId = lector.Id
            });
            await _db.SaveChangesAsync();

            _db.ModuleHistories.AddRange(
                from m in await _db.Modules.ToListAsync()
                where m.LectureId == lect
                select new ModuleHistory
                {
                    IsPassed = false,
                    LectureHistoryId = lectureHistory.Id,
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
                    LectureHistoryId = lectureHistory.Id
                });

            await _db.SaveChangesAsync();
        }

        public async Task<ActiveLectureViewModel> GetActiveLecture(Lector lector)
        {
            ActiveLectureViewModel activeLectureViewModel =
                await (from lh in _db.LecturesHistories
                       where lh.IsFrozen == false && lh.EndTime == null && lh.LectorId == lector.Id
                       join mh in _db.ModuleHistories on lh.Id equals mh.LectureHistoryId into mhjoin
                       join l in _db.Lectures on lh.LectureId equals l.Id
                       join d in _db.Disciplines on lh.DisciplineId equals d.Id
                       join m in _db.Modules on l.Id equals m.LectureId into mjoin
                       select new ActiveLectureViewModel()
                       {
                           LecturesHistory = lh,
                           Modules = mjoin,
                           ModuleHistories = mhjoin,
                           Lecture = l,
                           Discipline = d
                       }).SingleOrDefaultAsync();
            activeLectureViewModel.Lector = lector;
            return activeLectureViewModel;
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
            TimerAssociates.StartTimer(moduleHistoryId, minutesToPass, TimerAssociates.TimerType.RealtimeId);
            await _db.SaveChangesAsync();
        }

        public async Task ModulePassed(int moduleHistoryId)
        {
            ModuleHistory moduleHistory =
                await _db.ModuleHistories.SingleOrDefaultAsync(mh => mh.Id == moduleHistoryId);
            moduleHistory.IsPassed = true;
            await _db.SaveChangesAsync();
            QuizManager quizManager = new QuizManager();
            var lectureId =
                await (from l in _db.Lectures
                       join lh in _db.LecturesHistories on l.Id equals lh.LectureId
                       where lh.Id == moduleHistory.LectureHistoryId
                       select l).Select(l => l.Id).SingleOrDefaultAsync();
            foreach (var studentId in QuizHub.Students.GetStudents(moduleHistoryId))
            {
                await quizManager.ResovlePassedRealtimeQuiz(moduleHistory.ModuleId, studentId, moduleHistoryId, lectureId);
            }
            TimerAssociates.DisposeTimer(moduleHistoryId, TimerAssociates.TimerType.RealtimeId);
            quizManager.Dispose();
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}