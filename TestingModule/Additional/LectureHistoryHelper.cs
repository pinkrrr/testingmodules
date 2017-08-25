using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{
    public class LectureHistoryHelper
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public void StartLecture(ReasignViewModel model)
        {
            var disc = model.Disciplines[0].Id;
            var lect = model.Lectures[0].Id;
            var lecturesTable = _db.Set<LecturesHistory>();
            var date = DateTime.Now;
            lecturesTable.Add(new LecturesHistory() { LectureId = lect, DisciplineId = disc, StartTime = date, ModulesPassed = 0 });
            _db.SaveChanges();


            var groupsLectsTable = _db.Set<LectureHistoryGroup>();
            var lects = _db.LecturesHistories.ToList();
            int lectureHistoryId = lects.Where(t => t.StartTime == date).Select(t => t.Id).FirstOrDefault();
            foreach (var group in model.Groups.Where(t => t.IsSelected))
            {
                groupsLectsTable.Add(new LectureHistoryGroup() { LectureHistoryId = lectureHistoryId, GroupId = group.Id });
                _db.SaveChanges();
            }

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

        public void StartModule(int moduleId, string login)
        {
            var lector = _db.Accounts.FirstOrDefault(t => t.Login == login).Id;
            var lectorId = _db.Lectors.FirstOrDefault(t => t.AccountId == lector).Id;
            var lectorsDisciplines = _db.LectorDisciplines.Where(t => t.LectorId == lectorId).Select(t => t.DisciplineId)
                .ToList();
            var activeLectures = _db.LecturesHistories
                .Where(t => lectorsDisciplines.Contains(t.DisciplineId) && t.EndTime == null).FirstOrDefault().Id;
            if (_db.ModuleHistories.Where(t => t.LectureHistoryId == activeLectures).Any())
            {
               
            }
            else
            {
                var moduleHistTable = _db.Set<ModuleHistory>();
                moduleHistTable.Add(new ModuleHistory() { ModuleId = moduleId, LectureHistoryId = activeLectures });
                _db.SaveChanges();
            }

        }

        public void StopModule(int moduleId, string login)
        {
            var lector = _db.Accounts.FirstOrDefault(t => t.Login == login).Id;
            var lectorId = _db.Lectors.FirstOrDefault(t => t.AccountId == lector).Id;
            var lectorsDisciplines = _db.LectorDisciplines.Where(t => t.LectorId == lectorId).Select(t => t.DisciplineId)
                .ToList();
            var activeLectures = _db.LecturesHistories
                .Where(t => lectorsDisciplines.Contains(t.DisciplineId) && t.EndTime == null).FirstOrDefault().Id;
            _db.ModuleHistories.RemoveRange(_db.ModuleHistories.Where(t => t.LectureHistoryId == activeLectures && t.ModuleId == moduleId));
            _db.SaveChanges();
        }

    }
}