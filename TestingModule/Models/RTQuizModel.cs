using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestingModule.Additional;

namespace TestingModule.Models
{
    public class ResponseTable
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public int StudentId { get; set; }
        public int ResponseId { get; set; }
        public int LectureHistoryId { get; set; }
        public int GroupId { get; set; }
        public int ModuleId { get; set; }
    }

    public class AnswersForGroup
    {
        public string Text { get; set; }
        public int GroupId { get; set; }
        public int QuestionId { get; set; }
        public int Count { get; set; }
    }

    public class RealTimeStatistics
    {
        public int QuestionId { get; set; }
        public int TotalAnswers { get; set; }
        public int CorrectAnswers { get; set; }
        public int GroupId { get; set; }
    }


    
}