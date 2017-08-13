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
        //public Module Module { get; set; }
        public IEnumerable<Question> QuestionsList { get; set; }
        public Question Question { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
        public Respons Response { get; set; }
    }
}