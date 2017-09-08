using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class ResponseStatisticsViewModel
    {
        public Module Module { get; set; }
        //public ResponseTable ResponseTable { get; set; }
        public Question Question { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<Respons> Responses { get; set; }
        public IEnumerable<AnswersCount> AnswersCount { get; set; }
    }
}