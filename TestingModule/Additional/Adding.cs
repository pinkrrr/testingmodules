using System.Linq;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class Adding
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public string AddNewDiscipline(string name)
        {
            string result;
            var matches = _db.Disciplines.Count(t => t.Name == name);
            if (matches == 0)
            {
                var last = _db.Disciplines.OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefault() + 1;
                var disciplinesTable = _db.Set<Discipline>();
                disciplinesTable.Add(new Discipline() { Id = last, Name = name });
                _db.SaveChanges();
                result = "New skill has been successfully added to DB!";
            }
            else
            {
                result = "Current skill has been already created before! Try another one.";
            }
            return result;
        }
        public string AddNewLecture(string name, int disciplineId)
        {
            string result;
            var matches = _db.Lectures.Count(t => t.Name == name);
            if (matches == 0)
            {
                var last = _db.Lectures.OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefault() + 1;
                var lecturesTable = _db.Set<Lecture>();
                lecturesTable.Add(new Lecture() { Id = last, DisciplineId = disciplineId, Name = name });
                _db.SaveChanges();
                result = "New skill has been successfully added to DB!";
            }
            else
            {
                result = "Current skill has been already created before! Try another one.";
            }
            return result;
        }
        public string AddNewModule(string name, int lectureId, int disciplineId)
        {
            string result;
            var matches = _db.Modules.Count(t => t.Name == name);
            if (matches == 0)
            {
                var last = _db.Modules.OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefault() + 1;
                var lecturesTable = _db.Set<Module>();
                lecturesTable.Add(new Module() { Id = last, DisciplineId = disciplineId, LectureId = lectureId, Name = name });
                _db.SaveChanges();
                result = "New skill has been successfully added to DB!";
            }
            else
            {
                result = "Current skill has been already created before! Try another one.";
            }
            return result;
        }
        public string AddNewSpeciality(string name)
        {
            string result;
            var matches = _db.Specialities.Count(t => t.Name == name);
            if (matches == 0)
            {
                var last = _db.Specialities.OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefault() + 1;
                var specialityTable = _db.Set<Speciality>();
                specialityTable.Add(new Speciality() { Id = last, Name = name });
                _db.SaveChanges();
                result = "New skill has been successfully added to DB!";
            }
            else
            {
                result = "Current skill has been already created before! Try another one.";
            }
            return result;
        }
        public string AddNewGroup(string name, int specialityId)
        {
            string result;
            var matches = _db.Groups.Count(t => t.Name == name);
            if (matches == 0)
            {
                var last = _db.Groups.OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefault() + 1;
                var groupsTable = _db.Set<Group>();
                groupsTable.Add(new Group() { Id = last, SpecialityId = specialityId, Name = name });
                _db.SaveChanges();
                result = "New skill has been successfully added to DB!";
            }
            else
            {
                result = "Current skill has been already created before! Try another one.";
            }
            return result;
        }
        public string AddNewStudent(string name, string surname, int groupId, int specialityId)
        {
            UsernameAndPassword usernameAndPassword = new UsernameAndPassword();
            var username = usernameAndPassword.TranslitFileName(name.ToLower()) + "." +
                           usernameAndPassword.TranslitFileName(surname.ToLower());
            var password = usernameAndPassword.Password();
            var accountsTable = _db.Set<Account>();
            accountsTable.Add(new Account() { Login = username, Password = password, RoleId = 2 });
            _db.SaveChanges();

            string result;
            var matches = _db.Students.Count(t => t.Name == name);
            if (matches == 0)
            {
                var last = _db.Accounts.FirstOrDefault(t => t.Login == username);
                var studentsTable = _db.Set<Student>();
                studentsTable.Add(new Student() { SpecialityId = specialityId, GroupId = groupId, Name = name, Surname = surname, AccountId = last.Id });
                _db.SaveChanges();
                result = "New skill has been successfully added to DB!";
            }
            else
            {
                result = "Current skill has been already created before! Try another one.";
            }
            return result;
        }
    }
}