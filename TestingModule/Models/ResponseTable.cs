using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

    public class AnswersCount
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public int Count { get; set; }
    }
}