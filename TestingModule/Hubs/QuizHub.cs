﻿using System;
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
        private readonly testingDbEntities _context;
        private readonly QuizManager _quizManager;
        public static RealtimeQuizStudentMapping Students = new RealtimeQuizStudentMapping();
        
        public QuizHub()
        {
            _context = new testingDbEntities();
            _quizManager = new QuizManager(_context);
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

        public async Task<IndividualQuizViewModel> SaveIndividualResponse(IndividualQuizViewModel quizVM, int responseId)
        {
            if (await _context.ModuleHistories.AnyAsync(mh => mh.ModuleId == quizVM.IndividualQuizId && mh.IsPassed))
                return null;
            if (await _context.IndividualResponses.AnyAsync(r =>
                r.IndividualQuizId == quizVM.IndividualQuizId && r.StudentId == quizVM.Student.Id &&
                r.QuestionId == quizVM.Question.Id))
                return await _quizManager.GetIndividualQnA(quizVM.IndividualQuizId);
            IndividualRespons response = new IndividualRespons()
            {
                AnswerId = responseId,
                IndividualQuizId = quizVM.IndividualQuizId,
                ModuleId = quizVM.Question.ModuleId,
                QuestionId = quizVM.Question.Id,
                StudentId = quizVM.Student.Id
            };
            _context.IndividualResponses.Add(response);
            await _context.SaveChangesAsync();
            return await _quizManager.GetIndividualQnA(quizVM.IndividualQuizId);
        }

        public async Task<CumulativeQuizViewModel> SaveCumulativeResponse(CumulativeQuizViewModel quizVM, int responseId)
        {
            if (await _context.ModuleHistories.AnyAsync(mh => mh.ModuleId == quizVM.CumulativeQuizId && mh.IsPassed))
                return null;
            if (await _context.CumulativeResponses.AnyAsync(r =>
                r.CumulativeQuizId == quizVM.CumulativeQuizId && r.StudentId == quizVM.Student.Id &&
                r.QuestionId == quizVM.Question.Id))
                return await _quizManager.GetCumulativeQnA(quizVM.CumulativeQuizId);
            CumulativeRespons response = new CumulativeRespons()
            {
                AnswerId = responseId,
                CumulativeQuizId = quizVM.CumulativeQuizId,
                QuestionId = quizVM.Question.Id,
                StudentId = quizVM.Student.Id,
                LectureId = quizVM.Question.LectureId
            };
            _context.CumulativeResponses.Add(response);
            await _context.SaveChangesAsync();
            return await _quizManager.GetCumulativeQnA(quizVM.CumulativeQuizId);
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

        public void SendQVM(IEnumerable<int> students, int moduleHistoryId)
        {

            foreach (int student in Connections.Any(students))
            {
                foreach (string connection in Connections.GetConnections(student))
                {
                    Clients.Client(connection).ReciveModuleHistoryId(moduleHistoryId);
                }
            }
        }

        public static void StopModule(int moduleHistoryId)
        {
            var hubcontext = GlobalHost.ConnectionManager.GetHubContext<QuizHub>();
            foreach (var student in Students.GetStudents(moduleHistoryId))
            {
                foreach (string connection in Connections.GetConnections(student))
                {
                    hubcontext.Clients.Client(connection).RecieveStopModule();
                }
            }
        }

        private static readonly ConnectionMapping<int> Connections =
            new ConnectionMapping<int>();


        public override Task OnConnected()
        {
            if (Context.User.IsInRole(RoleName.Student))
            {
                int studentId = AccountCredentials.GetStudentId((ClaimsIdentity)Context.User.Identity);
                Connections.Add(studentId, Context.ConnectionId);
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (Context.User.IsInRole(RoleName.Student))
            {
                int studentId = AccountCredentials.GetStudentId((ClaimsIdentity)Context.User.Identity);
                Connections.Remove(studentId, Context.ConnectionId);
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            if (Context.User.IsInRole(RoleName.Student))
            {
                int studentId = AccountCredentials.GetStudentId((ClaimsIdentity)Context.User.Identity);
                if (!Connections.GetConnections(studentId).Contains(Context.ConnectionId))
                {
                    Connections.Add(studentId, Context.ConnectionId);
                }
            }

            return base.OnReconnected();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
                _quizManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}