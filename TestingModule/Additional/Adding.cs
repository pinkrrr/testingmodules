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
            var lecturesTable = _db.Set<Lecture>();
            lecturesTable.Add(new Lecture() { DisciplineId = disciplineId, Name = name });
            _db.SaveChanges();
        }
        public void AddNewModule(string name, int lectureId, int disciplineId, int minutes)
        {
            var lecturesTable = _db.Set<Module>();
            lecturesTable.Add(new Module() { DisciplineId = disciplineId, LectureId = lectureId, Name = name, MinutesToPass = minutes });
            _db.SaveChanges();
        }
        public void AddNewQuestion(string name, int lectureId, int disciplineId, int moduleId)
        {
            if (disciplineId == 0 || lectureId == 0)
            {
                lectureId = _db.Modules.FirstOrDefault(t => t.Id == moduleId).LectureId;
                disciplineId = _db.Modules.FirstOrDefault(t => t.Id == moduleId).DisciplineId;
            }
            var questionsTable = _db.Set<Question>();
            questionsTable.Add(new Question() { DisciplineId = disciplineId, LectureId = lectureId, ModuleId = moduleId, Text = name });
            _db.SaveChanges();
            var questionId = _db.Questions
                .FirstOrDefault(t => t.DisciplineId == disciplineId && t.LectureId == lectureId &&
                                    t.ModuleId == moduleId && t.Text == name).Id;
            var answersTable = _db.Set<Answer>();
            answersTable.Add(new Answer() { QuestionId = questionId, Text = "Не знаю відповіді", IsCorrect = false });
            _db.SaveChanges();
        }
        public void AddNewAnswer(string name, int questionId)
        {
            var answersTable = _db.Set<Answer>();
            if (answersTable.Count(t => t.QuestionId == questionId) == 1)
            {
                answersTable.Add(new Answer() { QuestionId = questionId, Text = name, IsCorrect = true });
            }
            else
            {
                answersTable.Add(new Answer() { QuestionId = questionId, Text = name, IsCorrect = false });
            }
            _db.SaveChanges();
        }
        public void AddNewSpeciality(string name)
        {
            var specialityTable = _db.Set<Speciality>();
            specialityTable.Add(new Speciality() { Name = name });
            _db.SaveChanges();
        }
        public void AddNewGroup(string name, int specialityId)
        {
            var groupsTable = _db.Set<Group>();
            groupsTable.Add(new Group() { SpecialityId = specialityId, Name = name });
            _db.SaveChanges();
        }
        public void AddNewStudent(string name, string surname, int groupId, int specialityId)
        {
            if (specialityId == 0)
            {
                specialityId = _db.Groups.FirstOrDefault(t => t.Id == groupId).SpecialityId;
            }
            UsernameAndPassword usernameAndPassword = new UsernameAndPassword();
            var group = _db.Groups.FirstOrDefault(t => t.Id == groupId).Name;
            var shortName = "";
            var numbers = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            foreach (var part in group.Split(' '))
            {
                if (numbers.Contains(part[0].ToString()))
                {
                    for (int i = 0; i < part.Length; i++)
                    {
                        if (numbers.Contains(part[i].ToString()))
                        {
                            shortName += part[i];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    shortName += part.Substring(0, 1);
                }
            }
            var username = usernameAndPassword.TranslitFileName(shortName.ToLower()) + "."
                + usernameAndPassword.TranslitFileName(name.ToLower()) + "." +
                           usernameAndPassword.TranslitFileName(surname.ToLower());
            var password = usernameAndPassword.Password();
            var accountsTable = _db.Set<Account>();
            accountsTable.Add(new Account() { Login = username, Password = password, RoleId = 2 });
            _db.SaveChanges();

            var last = _db.Accounts.FirstOrDefault(t => t.Login == username && t.Password == password);
            var studentsTable = _db.Set<Student>();
            studentsTable.Add(new Student() { SpecialityId = specialityId, GroupId = groupId, Name = name, Surname = surname, AccountId = last.Id });
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

        public void AddNewError(string url, string description)
        {
            var errorTable = _db.Set<ExeptionLog>();
            errorTable.Add(new ExeptionLog() { Url = url, Description = description, Date = DateTime.Now.ToString("MM_dd_yyyy_H_mm_ss"), Resolved = false });
            _db.SaveChanges();
        }
    }
}