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
        public ResponseTable ResponseTable { get; set; }
        public Question Question { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
        public Group Group { get; set; }
        public Respons Response { get; set; }
    }
}