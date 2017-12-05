using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class StatisticsViewModel
    {
        public Lector Lector { get; set; }
        public IEnumerable<Discipline> Disciplines { get; set; }
        public IEnumerable<Lecture> Lectures { get; set; }
        public IEnumerable<LecturesHistory> Histories { get; set; }
    }

    public class ResponseStatisticsViewModel
    {
        public IEnumerable<Module> Modules { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<AnswersForGroup> AnswersCount { get; set; }
    }

    public class RealTimeStatisticsViewModel
    {
        public Lector Lector { get; set; }
        public LecturesHistory LecturesHistory { get; set; }
        public ModuleHistory ModuleHistory { get; set; }
        public Module Module { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public double TimeLeft { get; set; }

    }
}