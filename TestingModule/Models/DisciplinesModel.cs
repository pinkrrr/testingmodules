using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestingModule.Models
{
    public class DisciplinesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisciplineId { get; set; }
        public int LectureId { get; set; }
        public int ModuleId { get; set; }
    }
}