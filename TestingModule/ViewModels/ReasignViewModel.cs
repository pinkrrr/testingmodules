using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class ReasignViewModel
    {
        public IEnumerable<Discipline> Disciplines { get; set; }
        public IEnumerable<Lecture> Lectures { get; set; }
        public IEnumerable<Module> Modules { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
        public IEnumerable<Speciality> Specialities { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<StudentDiscipline> StudentDisciplines { get; set; }
    }
}