using System;
using System.Collections.Generic;
using System.EnterpriseServices;
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
                LectureHistoryId = quizVM.LectureHistoryId,
                ModuleHistoryId = quizVM.ModuleHistoryId,
                QuestionId = quizVM.Question.Id,
                StudentId = quizVM.Student.Id,
                GroupId = quizVM.Student.GroupId
            };
            _context.Respons.Add(response);
            await _context.SaveChangesAsync();
            Clients.All.ResponseRecieved();
            await _quizManager.UpdateQuizModel(quizVM);
            return quizVM;
        }

        private static bool _locked;
        public async Task QueryRealTimeStats(RealTimeStatisticsViewModel realTimeStatisticsVM,bool immediateCheck)
        {
            if (!_locked)
            {
                _locked = true;
                if(!immediateCheck) await Task.Delay(5000);
                IEnumerable<RealTimeStatistics> realTimeStatistics =
                    _quizManager.GetRealTimeStatistics(realTimeStatisticsVM).ToList();
                Clients.Caller.RecieveRealTimeStatistics(realTimeStatistics);
                _locked = false;
                }
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
        }

        public void StopModule()
        {
            Clients.All.reciveStopModule();
        }

    }

    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
        public static string GroupName { get; set; }
    }

}