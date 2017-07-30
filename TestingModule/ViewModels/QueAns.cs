using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestingModule.Models
{
    public class  QueAns
    {
        public int QuestionId { get; set; }
        public int DisciplineId { get; set; }
        public int LectureId { get; set; }
        public int ModuleId { get; set; }
        public string Question { get; set; }
        public int? AnswerId { get; set; }
        public String Answer { get; set; }
        public bool? IsCorrect { get; set; }
    }
}