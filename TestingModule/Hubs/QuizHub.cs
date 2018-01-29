using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;
using System.Web.UI.WebControls;
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
        private testingDbEntities _context;
        private QuizManager _quizManager;

        public QuizHub()
        {
            _context = new testingDbEntities();
            _quizManager = new QuizManager();
        }

        public async Task<RealTimeQuizViewModel> SaveResponse(RealTimeQuizViewModel quizVM, int responseId)
        {
            if (await _context.ModuleHistories.AnyAsync(mh => mh.ModuleId == quizVM.ModuleHistoryId && mh.IsPassed))
                return null;
            if (await _context.RealtimeResponses.AnyAsync(r =>
                r.ModuleHistoryId == quizVM.ModuleHistoryId && r.StudentId == quizVM.Student.Id &&
                r.QuestionId == quizVM.Question.Id))
                return await _quizManager.UpdateQuizModel(quizVM);
            RealtimeRespons response = new RealtimeRespons
            {
                AnswerId = responseId,
                LectureHistoryId = quizVM.LectureHistoryId,
                ModuleHistoryId = quizVM.ModuleHistoryId,
                QuestionId = quizVM.Question.Id,
                StudentId = quizVM.Student.Id,
                GroupId = quizVM.Student.GroupId
            };
            _context.RealtimeResponses.Add(response);
            await _context.SaveChangesAsync();
            Clients.All.ResponseRecieved();
            return await _quizManager.UpdateQuizModel(quizVM);
        }

        private static bool _locked;
        public async Task QueryRealTimeStats(RealTimeStatisticsViewModel realTimeStatisticsVM, bool immediateCheck)
        {
            if (!_locked)
            {
                _locked = true;
                if (!immediateCheck) await Task.Delay(5000);
                IEnumerable<RealTimeStatistics> realTimeStatistics =
                    _quizManager.GetRealTimeStatistics(realTimeStatisticsVM).ToList();
                Clients.Caller.RecieveRealTimeStatistics(realTimeStatistics);
                _locked = false;
            }
        }

        private static readonly ConnectionMapping<string> Connections =
            new ConnectionMapping<string>();
        public override Task OnConnected()
        {
            if (Context.User.IsInRole(RoleName.Student))
            {
                string group = new AccountCredentials().GetStudentGroup((ClaimsIdentity)Context.User.Identity);
                Connections.Add(group, Context.ConnectionId);
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (Context.User.IsInRole(RoleName.Student))
            {
                string group = new AccountCredentials().GetStudentGroup((ClaimsIdentity)Context.User.Identity);
                Connections.Remove(group, Context.ConnectionId);
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            if (Context.User.IsInRole(RoleName.Student))
            {
                string group = new AccountCredentials().GetStudentGroup((ClaimsIdentity)Context.User.Identity);
                if (!Connections.GetConnections(group).Contains(Context.ConnectionId))
                {
                    Connections.Add(group, Context.ConnectionId);
                }
            }

            return base.OnReconnected();
        }

        public void SendQVM(IEnumerable<string> groups, int moduleHistoryId)
        {

            foreach (string group in Connections.Any(groups))
            {
                foreach (string connection in Connections.GetConnections(group))
                {
                    Clients.Client(connection).ReciveModuleHistoryId(moduleHistoryId);
                }
            }
        }

        public void StopModule()
        {
            Clients.All.reciveStopModule();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context = null;
                _quizManager = null;
            }
            base.Dispose(disposing);
        }
    }
}