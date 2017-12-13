using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class QuizViewModel
    {
        public Student Student { get; set; }
        //public ICollection<Question> QuestionsList { get; set; }
        public Question Question { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
        public int LectureHistoryId { get; set; }
        public int ModuleHistoryId { get; set; }
    }
}