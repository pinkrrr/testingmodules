using System;
using System.Collections.Generic;
using System.Linq;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{
    public class Adding
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public void AddNewDiscipline(string name, int? lectorId)
        {
            var disciplinesTable = _db.Set<Discipline>();
            disciplinesTable.Add(new Discipline() { Name = name });
            _db.SaveChanges();
            var disciplineId = _db.Disciplines.Where(t => t.Name == name).FirstOrDefault().Id;
            var lecturesTable = _db.Set<LectorDiscipline>();
            lecturesTable.Add(new LectorDiscipline() { LectorId = Convert.ToInt32(lectorId), DisciplineId = disciplineId });
            _db.SaveChanges();
        }
        public void AddNewLecture(string name, int disciplineId)
        {
            var last = _db.Lectures.OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefault() + 1;
            var lecturesTable = _db.Set<Lecture>();
            lecturesTable.Add(new Lecture() { Id = last, DisciplineId = disciplineId, Name = name });
            _db.SaveChanges();
        }
        public void AddNewModule(string name, int lectureId, int disciplineId)
        {
            var last = _db.Modules.OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefault() + 1;
            var lecturesTable = _db.Set<Module>();
            lecturesTable.Add(new Module() { Id = last, DisciplineId = disciplineId, LectureId = lectureId, Name = name });
            _db.SaveChanges();
        }
        public void AddNewQuestion(string name, int lectureId, int disciplineId, int moduleId)
        {
            var questionsTable = _db.Set<Question>();
            questionsTable.Add(new Question() { DisciplineId = disciplineId, LectureId = lectureId, ModuleId = moduleId, Text = name });
            _db.SaveChanges();
        }
        public void AddNewAnswer(string name, int questionId)
        {
            var questionsTable = _db.Set<Answer>();
            questionsTable.Add(new Answer() { QuestionId = questionId, Text = name });
            _db.SaveChanges();
        }
        public void AddNewSpeciality(string name)
        {
            var last = _db.Specialities.OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefault() + 1;
            var specialityTable = _db.Set<Speciality>();
            specialityTable.Add(new Speciality() { Id = last, Name = name });
            _db.SaveChanges();
        }
        public void AddNewGroup(string name, int SpecialityId)
        {
            var last = _db.Groups.OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefault() + 1;
            var groupsTable = _db.Set<Group>();
            groupsTable.Add(new Group() { Id = last, SpecialityId = SpecialityId, Name = name });
            _db.SaveChanges();
        }
        public void AddNewStudent(string name, string surname, int GroupId, int SpecialityId)
        {
            UsernameAndPassword usernameAndPassword = new UsernameAndPassword();
            var username = usernameAndPassword.TranslitFileName(name.ToLower()) + "." +
                           usernameAndPassword.TranslitFileName(surname.ToLower());
            var password = usernameAndPassword.Password();
            var accountsTable = _db.Set<Account>();
            accountsTable.Add(new Account() { Login = username, Password = password, RoleId = 2 });
            _db.SaveChanges();

            var last = _db.Accounts.FirstOrDefault(t => t.Login == username && t.Password == password);
            var studentsTable = _db.Set<Student>();
            studentsTable.Add(new Student() { SpecialityId = SpecialityId, GroupId = GroupId, Name = name, Surname = surname, AccountId = last.Id });
            _db.SaveChanges();
        }

        public void AddNewLector(string name, string surname)
        {
            UsernameAndPassword usernameAndPassword = new UsernameAndPassword();
            var username = usernameAndPassword.TranslitFileName(name.ToLower()) + "." +
                           usernameAndPassword.TranslitFileName(surname.ToLower());
            var password = usernameAndPassword.Password();
            var accountsTable = _db.Set<Account>();
            accountsTable.Add(new Account() { Login = username, Password = password, RoleId = 3 });
            _db.SaveChanges();

            var last = _db.Accounts.FirstOrDefault(t => t.Login == username);
            var lectorsTable = _db.Set<Lector>();
            lectorsTable.Add(new Lector() { Name = name, Surname = surname, AccountId = last.Id });
            _db.SaveChanges();
        }

        public void AddNewStudentConnection(ReasignViewModel model)
        {
            try
            {
                var connectionTable = _db.Set<StudentDiscipline>();
                var disc = model.StudentDisciplines.FirstOrDefault().DisciplineId;
                var students = _db.StudentDisciplines
                    .Where(t => t.DisciplineId == disc)
                    .Select(t => t.StudentId).ToList();
                foreach (var item in model.StudentDisciplines.Where(t => t.StudentId != null))
                {
                    if (item.IsSelected)
                    {
                        if (!students.Contains(item.StudentId))
                        {
                            connectionTable.Add(new StudentDiscipline() { DisciplineId = item.DisciplineId, StudentId = item.StudentId });
                        }
                    }
                    else
                    {
                        if (students.Contains(item.StudentId))
                        {
                            connectionTable.Remove(_db.StudentDisciplines.FirstOrDefault(t => t.DisciplineId == item.DisciplineId && t.StudentId == item.StudentId));
                        }
                    }
                }
                _db.SaveChanges();
            }
            catch (Exception)
            {

            }
        }
    }
}