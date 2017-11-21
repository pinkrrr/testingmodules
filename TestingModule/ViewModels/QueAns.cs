using System;
using System.Collections.Generic;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class QueAns
    {
        public int QuestionId { get; set; }
        public int DisciplineId { get; set; }
        public int LectureId { get; set; }
        public int ModuleId { get; set; }
        public string Description { get; set; }
        public string Question { get; set; }
        public int? AnswerId { get; set; }
        public String Answer { get; set; }
        public bool? IsCorrect { get; set; }
        public IEnumerable<Module> Modules { get; set; }
        public int CorrectAnswerId { get; set; }
        public int QuestionType { get; set; }
    }
}