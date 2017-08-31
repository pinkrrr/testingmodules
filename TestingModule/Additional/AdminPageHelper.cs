using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        public ReasignViewModel LecturesIndexPage(ClaimsIdentity claimsIdentity)
        {
            var db = new testingDbEntities();
            ReasignViewModel model = new ReasignViewModel();
            var login = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value.ToString();
            var role = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Role).Value.ToString();

            if (role == "Lecturer")
            {
                var lector = db.Accounts.FirstOrDefault(t => t.Login == login).Id;
                var lectorId = db.Lectors.FirstOrDefault(t => t.AccountId == lector).Id;
                var lectorsDisciplines = db.LectorDisciplines.Where(t => t.LectorId == lectorId).Select(t => t.DisciplineId)
                    .ToList();
                var students = db.StudentDisciplines.Where(t => lectorsDisciplines.Contains(t.DisciplineId))
                    .Select(t => t.StudentId).ToList();
                var groups = db.Students.Where(t => students.Contains(t.Id)).Select(t => t.GroupId).ToList();
                model.Disciplines = db.Disciplines.Where(t => lectorsDisciplines.Contains(t.Id)).ToList();
                model.Modules = db.Modules.Where(t => lectorsDisciplines.Contains(t.DisciplineId)).ToList();
                model.Lectures = db.Lectures.Where(t => lectorsDisciplines.Contains(t.DisciplineId)).ToList();
                model.Groups = db.Groups.Where(t => groups.Contains(t.Id)).ToList();
                model.LecturesHistories = db.LecturesHistories.Where(t => t.StartTime != null && t.EndTime == null)
                    .ToList();
                model.ModuleHistories = db.ModuleHistories.ToList();
                var startedLectures = db.LecturesHistories
                    .Where(t => lectorsDisciplines.Contains(t.Id) && t.EndTime == null).ToList();
                if (startedLectures.Any())
                {
                    model.LecturesHistories = startedLectures;
                }
                return model;
            }
            return null;
        }

        
    }
}