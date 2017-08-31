using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;
using Microsoft.AspNet.SignalR;
using TestingModule.Additional;
using TestingModule.Models;
using TestingModule.ViewModels;
using Newtonsoft.Json;

namespace TestingModule.Hubs
{
    [Authorize]
    public class QuizHub : Hub
    {
        private testingDbEntities _context = new testingDbEntities();
        private QuizManager _quizManager = new QuizManager();

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
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public async Task<QuizViewModel> SaveResponse(QuizViewModel quizVM, int responseId)
        {
            //await OnConnected();
            Respons response = new Respons
            {
                AnswerId = responseId,
                LectureHistoryId = quizVM.Question.LectureId,
                QuestionId = quizVM.Question.Id
            };
            _context.Respons.Add(response);
            await _context.SaveChangesAsync();

            if (quizVM.Answers.Where(a => a.Id == responseId).Select(a => a.IsCorrect).SingleOrDefault()==true)
            {
                Clients.All.RecieveStatistics(response.QuestionId, UserHandler.ConnectedIds.Count);
            }
            else
            {
                Clients.All.RecieveStatistics(null, UserHandler.ConnectedIds.Count);
            }
            await _quizManager.UpdateQuizModel(quizVM);
            return quizVM;
        }

        public void ModuleEnquire()
        {
            var claimsIdentity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            int accountId = Int32.Parse(claimsIdentity.Claims.Where(c => c.Type == "Id")
                .Select(c => c.Value)
                .SingleOrDefault());
            Clients.All.RecieveEnquire(accountId, Context.ConnectionId);
        }

        public void SendQVM(int moduleId, string connectionId)
        {
            Clients.Client(connectionId).ReciveModuleId(moduleId);
            //return qvm;
        }

    }

    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
        public static string GroupName { get; set; }
    }
}