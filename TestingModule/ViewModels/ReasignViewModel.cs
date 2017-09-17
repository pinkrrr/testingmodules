using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class ReasignViewModel
    {
        public IList<Discipline> Disciplines { get; set; }
        public IList<Lecture> Lectures { get; set; }
        public IList<Module> Modules { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
        public IEnumerable<Speciality> Specialities { get; set; }
        public IList<Group> Groups { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<LecturesHistory> LecturesHistories { get; set; }
        public IList<ModuleHistory> ModuleHistories { get; set; }
        public IList<StudentDiscipline> StudentDisciplines { get; set; }
        public Lector Lector { get; set; }
    }
}