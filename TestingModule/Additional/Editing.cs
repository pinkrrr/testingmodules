using System;
using System.Linq;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class Editing
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public void EditDiscipline(int disciplineId, string name, int? lectorId)
        {
            var disc = _db.Disciplines.FirstOrDefault(t => t.Id == disciplineId);
            if (_db.LectorDisciplines.Any(t => t.DisciplineId == disciplineId))
            {
                var connect = _db.LectorDisciplines.FirstOrDefault(t => t.DisciplineId == disciplineId);
                connect.LectorId = Convert.ToInt32(lectorId);
                _db.SaveChanges();
            }
            else
            {
                var lecturesTable = _db.Set<LectorDiscipline>();

                lecturesTable.Add(new LectorDiscipline() { LectorId = Convert.ToInt32(lectorId), DisciplineId = disciplineId });
            }
            disc.Name = name;
            _db.SaveChanges();
        }
        public void EditLecture(int lectureId, string name)
        {
            var lct = _db.Lectures.FirstOrDefault(t => t.Id == lectureId);
            lct.Name = name;
            _db.SaveChanges();
        }
        public void EditModule(int moduleId, string name)
        {
            var mdl = _db.Modules.FirstOrDefault(t => t.Id == moduleId);
            mdl.Name = name;
            _db.SaveChanges();
        }
        public void EditQuestion(int questionId, string text)
        {
            var qs = _db.Questions.FirstOrDefault(t => t.Id == questionId);
            qs.Text = text;
            _db.SaveChanges();
        }
        public void EditAnswer(int? answerId, string text, bool? isCorrect)
        {
            if (isCorrect == null)
            {
                isCorrect = false;
            }
            var anw = _db.Answers.FirstOrDefault(t => t.Id == answerId);
            anw.Text = text;
            anw.IsCorrect = isCorrect;
            _db.SaveChanges();
        }

        public void EditSpeciality(int specialityId, string name)
        {
            var spc = _db.Specialities.FirstOrDefault(t => t.Id == specialityId);
            spc.Name = name;
            _db.SaveChanges();
        }
        public void EditGroup(int groupId, string name)
        {
            var grp = _db.Groups.FirstOrDefault(t => t.Id == groupId);
            grp.Name = name;
            _db.SaveChanges();
        }
        public void EditStudent(int studentId, string name, string surname, string username, string pass)
        {
            var std = _db.Students.FirstOrDefault(t => t.Id == studentId);
            var ac = _db.Accounts.FirstOrDefault(t => t.Id == std.AccountId);
            std.Name = name;
            std.Surname = surname;
            ac.Login = username;
            ac.Password = pass;
            _db.SaveChanges();
        }
        public void EditLector(int lectorId, string name, string surname, string username, string pass)
        {
            var lct = _db.Lectors.FirstOrDefault(t => t.Id == lectorId);
            var ac = _db.Accounts.FirstOrDefault(t => t.Id == lct.AccountId);
            lct.Name = name;
            lct.Surname = surname;
            ac.Login = username;
            ac.Password = pass;
            _db.SaveChanges();
        }
    }
}