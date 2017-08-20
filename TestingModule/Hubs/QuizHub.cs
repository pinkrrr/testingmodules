using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using TestingModule.Additional;
using TestingModule.Models;
using TestingModule.ViewModels;

namespace TestingModule.Hubs
{
    public class QuizHub : Hub
    {
        private testingDbEntities _context = new testingDbEntities();

        public async void SaveResponse(QuizViewModel quizVM, int responseId)
        {
            Respons response = new Respons
            {
                AnswerId = responseId,
                LectureId = quizVM.Question.LectureId,
                QuestionId = quizVM.Question.Id
            };
            _context.Responses.Add(response);
            await _context.SaveChangesAsync();
            Question questionToRemove = quizVM.QuestionsList.SingleOrDefault(ql => ql.Id == quizVM.Question.Id);
            quizVM.QuestionsList.Remove(questionToRemove);
            //return quizVM;
            Clients.Caller.SaveCallerResponse(quizVM);
        }
    }
}