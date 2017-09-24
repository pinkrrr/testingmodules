using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TestingModule.Controllers;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{
    public class AdminPageHelper : AdminController
    {
        private testingDbEntities _db = new testingDbEntities();
        public async Task<ReasignViewModel> LecturesIndexPage(Lector lector)
        {

            ReasignViewModel model = new ReasignViewModel();
            model.Lector = lector;
            var lectorsDisciplines = await _db.LectorDisciplines.Where(t => t.LectorId == model.Lector.Id).Select(t => t.DisciplineId)
                .ToListAsync();
            var students = await _db.StudentDisciplines.Where(t => lectorsDisciplines.Contains(t.DisciplineId))
                .Select(t => t.StudentId).ToListAsync();
            var groups = await _db.Students.Where(t => students.Contains(t.Id)).Select(t => t.GroupId).ToListAsync();
            model.Disciplines = await _db.Disciplines.Where(t => lectorsDisciplines.Contains(t.Id)).ToListAsync();
            model.Modules = await _db.Modules.Where(t => lectorsDisciplines.Contains(t.DisciplineId)).ToListAsync();
            model.Lectures = await _db.Lectures.Where(t => lectorsDisciplines.Contains(t.DisciplineId)).ToListAsync();
            model.Groups = await _db.Groups.Where(t => groups.Contains(t.Id)).ToListAsync();
            model.LecturesHistories = await _db.LecturesHistories.Where(t => t.EndTime == null && t.LectorId == model.Lector.Id)
                .ToListAsync();
            model.ModuleHistories =
                (from mh in await _db.ModuleHistories.ToListAsync()
                 join lh in model.LecturesHistories on mh.LectureHistoryId equals lh.Id
                 select mh).ToList();
            return model;
        }
    }
}