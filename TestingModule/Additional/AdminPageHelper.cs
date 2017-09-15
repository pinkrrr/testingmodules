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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TestingModule.Controllers;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{
    public class AdminPageHelper : adminController
    {
        public async Task<ReasignViewModel> LecturesIndexPage()
        {
            var db = new testingDbEntities();
            ReasignViewModel model = new ReasignViewModel();
            var role = new AccountCredentials().GetRole();

            if (role == RoleName.Lecturer)
            {
                model.Lector = await new AccountCredentials().GetLector();
                var lectorsDisciplines = await db.LectorDisciplines.Where(t => t.LectorId == model.Lector.Id).Select(t => t.DisciplineId)
                    .ToListAsync();
                var students = await db.StudentDisciplines.Where(t => lectorsDisciplines.Contains(t.DisciplineId))
                    .Select(t => t.StudentId).ToListAsync();
                var groups = await db.Students.Where(t => students.Contains(t.Id)).Select(t => t.GroupId).ToListAsync();
                model.Disciplines = await db.Disciplines.Where(t => lectorsDisciplines.Contains(t.Id)).ToListAsync();
                model.Modules = await db.Modules.Where(t => lectorsDisciplines.Contains(t.DisciplineId)).ToListAsync();
                model.Lectures = await db.Lectures.Where(t => lectorsDisciplines.Contains(t.DisciplineId)).ToListAsync();
                model.Groups = await db.Groups.Where(t => groups.Contains(t.Id)).ToListAsync();
                model.LecturesHistories = await db.LecturesHistories.Where(t => t.EndTime == null && t.LectorId == model.Lector.Id)
                    .ToListAsync();
                model.ModuleHistories =
                    (from mh in await db.ModuleHistories.ToListAsync()
                     join lh in model.LecturesHistories on mh.LectureHistoryId equals lh.Id
                     select mh).ToList();
                /*var startedLectures = db.LecturesHistories
                    .Where(t => lectorsDisciplines.Contains(t.Id) && t.EndTime == null).ToList();
                if (startedLectures.Any())
                {
                    model.LecturesHistories = startedLectures;
                }*/
                return model;
            }
            return null;
        }


    }
}