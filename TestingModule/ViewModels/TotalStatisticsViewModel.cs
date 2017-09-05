using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class TotalStatisticsViewModel
    {
        public Lector Lector { get; set; }
        public IEnumerable<Discipline> Disciplines { get; set; }
        public IEnumerable<Lecture> Lectures { get; set; }
        public IEnumerable<Module> Modules { get; set; }
        public IEnumerable<Group> Groups { get; set; }
    }
}