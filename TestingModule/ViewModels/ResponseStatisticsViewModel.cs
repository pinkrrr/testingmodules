using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class ResponseStatisticsViewModel
    {
        public IEnumerable<Module> Modules { get; set; }
        //public ResponseTable ResponseTable { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        //public IEnumerable<Answer> Answers { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        //public IEnumerable<Respons> Responses { get; set; }
        public IEnumerable<AnswersForGroup> AnswersCount { get; set; }
    }
}