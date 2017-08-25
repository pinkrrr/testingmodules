﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using TestingModule.Additional;
using TestingModule.Models;
using TestingModule.ViewModels;
using Newtonsoft.Json;

namespace TestingModule.Hubs
{
    public class QuizHub : Hub
    {
        private testingDbEntities _context = new testingDbEntities();
        private QuizManager quizManager = new QuizManager();

        public async Task<QuizViewModel> SaveResponse(QuizViewModel quizVM, int responseId)
        {
            await OnConnected();
            Respons response = new Respons
            {
                AnswerId = responseId,
                LectureId = quizVM.Question.LectureId,
                QuestionId = quizVM.Question.Id
            };
            _context.Responses.Add(response);
            await _context.SaveChangesAsync();
            await quizManager.UpdateQuizModel(quizVM);
            if (await quizManager.IsAnswerCorrect(response.AnswerId))
            {
                Clients.All.recieveStatistics(response.QuestionId, UserHandler.ConnectedIds.Count);
            }
            return quizVM;

        }

        public override Task OnConnected()
        {
            var role = new AccountCredentials().GetRole();
            if (role == RoleName.Student)
            {
                UserHandler.ConnectedIds.Add(Context.ConnectionId);
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var role = new AccountCredentials().GetRole();
            if (role == RoleName.Student)
            {
                UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            }
            return base.OnDisconnected(stopCalled);
        }
    }

    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }
}