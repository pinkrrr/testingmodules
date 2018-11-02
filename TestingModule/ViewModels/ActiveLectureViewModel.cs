using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class ActiveLectureViewModel
    {
        public Lector Lector { get; set; }
        public Discipline Discipline { get; set; }
        public Lecture Lecture { get; set; }
        public LecturesHistory LecturesHistory { get; set; }
        public IEnumerable<Module> Modules { get; set; }
        public IEnumerable<ModuleHistory> ModuleHistories { get; set; }
    }
}