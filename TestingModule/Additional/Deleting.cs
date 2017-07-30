using System.Data.Entity;
using System.Linq;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class Deleting
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public void DeleteDiscipline(int disciplineId)
        {
            var lectures = _db.Lectures.Where(t => t.DisciplineId == disciplineId).Select(t => t.Id).ToList();
            foreach (var lecture in lectures)
            {
                var modules = _db.Modules.Where(t => t.LectureId == lecture).Select(t => t.Id).ToList();
                foreach (var module in modules)
                {
                    var questions = _db.Questions.Where(t => t.ModuleId == module).Select(t => t.Id).ToList();
                    foreach (var que in questions)
                    {
                        _db.Answers.RemoveRange(_db.Answers.Where(t => t.QuestionId == que));
                    }
                    _db.Questions.RemoveRange(_db.Questions.Where(t => t.ModuleId == module));
                }
                _db.Modules.RemoveRange(_db.Modules.Where(t => t.LectureId == lecture));
            }
            _db.Lectures.RemoveRange(_db.Lectures.Where(t => t.DisciplineId == disciplineId));
            var disc = new Discipline() { Id = disciplineId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteLecture(int lectureId)
        {
            var modules = _db.Modules.Where(t => t.LectureId == lectureId).Select(t => t.Id).ToList();
            foreach (var module in modules)
            {
                var questions = _db.Questions.Where(t => t.ModuleId == module).Select(t => t.Id).ToList();
                foreach (var que in questions)
                {
                    _db.Answers.RemoveRange(_db.Answers.Where(t => t.QuestionId == que));
                }
                _db.Questions.RemoveRange(_db.Questions.Where(t => t.ModuleId == module));
            }

            var lec = new Lecture() { Id = lectureId };
            _db.Entry(lec).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteModule(int moduleId)
        {
            var questions = _db.Questions.Where(t => t.ModuleId == moduleId).Select(t => t.Id).ToList();
            foreach (var que in questions)
            {
                _db.Answers.RemoveRange(_db.Answers.Where(t => t.QuestionId == que));
            }
            _db.Questions.RemoveRange(_db.Questions.Where(t => t.ModuleId == moduleId));
            var mod = new Module() { Id = moduleId };
            _db.Entry(mod).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteQuestion(int questionId)
        {
            _db.Answers.RemoveRange(_db.Answers.Where(t => t.QuestionId == questionId));
            var que = new Question() { Id = questionId };
            _db.Entry(que).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteAnswer(int answerId)
        {
            var ans = new Answer() { Id = answerId };
            _db.Entry(ans).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteSpeciality(int specialityId)
        {
            var groups = _db.Groups.Where(t => t.SpecialityId == specialityId).Select(t => t.Id).ToList();
            foreach (var group in groups)
            {
                var students = _db.Students.Where(t => t.GroupId == group).Select(t => t.Id).ToList();
                foreach (var student in students)
                {
                    var id = _db.Students.Where(t => t.Id == student).Select(t => t.AccountId).FirstOrDefault();
                    _db.Accounts.RemoveRange(_db.Accounts.Where(t => t.Id == id));
                }
                _db.Students.RemoveRange(_db.Students.Where(t => t.GroupId == group));
            }
            _db.Groups.RemoveRange(_db.Groups.Where(t => t.SpecialityId == specialityId));
            var spc = new Speciality() { Id = specialityId };
            _db.Entry(spc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteGroup(int groupId)
        {
            var students = _db.Students.Where(t => t.GroupId == groupId).Select(t => t.Id).ToList();
            foreach (var student in students)
            {
                var id = _db.Students.Where(t => t.Id == student).Select(t => t.AccountId).FirstOrDefault();
                _db.Accounts.RemoveRange(_db.Accounts.Where(t => t.Id == id));
            }
            _db.Students.RemoveRange(_db.Students.Where(t => t.GroupId == groupId));
            var grp = new Group() { Id = groupId };
            _db.Entry(grp).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteStudent(int studentId)
        {
            var std = new Student() { Id = studentId };
            var ac = new Account() { Id = _db.Students.Where(t => t.Id == studentId).Select(t => t.AccountId).FirstOrDefault() };
            _db.Entry(std).State = EntityState.Deleted;
            _db.Entry(ac).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteLector(int lectorId)
        {
            var lct = new Lector() { Id = lectorId };
            var ac = new Account() { Id = _db.Lectors.Where(t => t.Id == lectorId).Select(t => t.AccountId).FirstOrDefault() };
            _db.Entry(lct).State = EntityState.Deleted;
            _db.Entry(ac).State = EntityState.Deleted;
            _db.SaveChanges();
        }

    }
}