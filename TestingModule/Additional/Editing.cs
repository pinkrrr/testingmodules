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
                if (connect != null) connect.LectorId = Convert.ToInt32(lectorId);
                _db.SaveChanges();
            }
            else
            {
                var lecturesTable = _db.Set<LectorDiscipline>();

                lecturesTable.Add(new LectorDiscipline() { LectorId = Convert.ToInt32(lectorId), DisciplineId = disciplineId });
            }
            if (disc != null) disc.Name = name;
            _db.SaveChanges();
        }
        public void EditLecture(int lectureId, string name, int disciplineId)
        {
            var lct = _db.Lectures.FirstOrDefault(t => t.Id == lectureId);
            if (disciplineId != _db.Lectures.FirstOrDefault(t => t.Id == lectureId).DisciplineId)
            {
                lct.DisciplineId = disciplineId;
            }
            lct.Name = name;
            _db.SaveChanges();
        }
        public void EditModule(int moduleId, string name, int lectureId)
        {
            var mdl = _db.Modules.FirstOrDefault(t => t.Id == moduleId);
            if (lectureId != _db.Modules.FirstOrDefault(t => t.Id == moduleId).LectureId)
            {
                mdl.LectureId = lectureId;
            }
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
        public void EditGroup(int groupId, string name, int specialityId)
        {
            var grp = _db.Groups.FirstOrDefault(t => t.Id == groupId);
            if (specialityId != _db.Groups.FirstOrDefault(t => t.Id == groupId).SpecialityId)
            {
                grp.SpecialityId = specialityId;
            }
            grp.Name = name;
            _db.SaveChanges();
        }
        public void EditStudent(int studentId, string name, string surname, string username, string pass, int groupId)
        {
            var std = _db.Students.FirstOrDefault(t => t.Id == studentId);
            var ac = _db.Accounts.FirstOrDefault(t => t.Id == std.AccountId);
            if (groupId != _db.Students.FirstOrDefault(t => t.Id == studentId).SpecialityId)
            {
                std.GroupId = groupId;
            }
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