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


    public class TimerAssociates
    {
        private readonly Timer timer;
        private readonly DateTime moduleFinish;
        private static readonly Dictionary<int, TimerAssociates> ModuleTimers = new Dictionary<int, TimerAssociates>();

        public TimerAssociates(Timer timer, DateTime moduleFinish)
        {
            this.timer = timer;
            this.moduleFinish = moduleFinish;
        }

        public TimerAssociates() { }

        public void StartModuleTimer(int moduleHistoryId, TimeSpan minutesToPass)
        {
            TimerAssociates timerAssociatives = new TimerAssociates(new Timer(StopModuleOnTimer, moduleHistoryId, minutesToPass, TimeSpan.Zero), DateTime.UtcNow + minutesToPass);
            ModuleTimers.Add(moduleHistoryId, timerAssociatives);
        }

        public async Task OnStartModuleTimer()
        {
            testingDbEntities db = new testingDbEntities();
            if (await db.ModuleHistories.AnyAsync(mh => mh.StartTime != null && mh.IsPassed != true))
            {
                IEnumerable<ModuleHistory> ongoingModules =
                    await (from mh in db.ModuleHistories
                           where mh.StartTime != null && mh.IsPassed != true
                           select mh).ToListAsync();
                IEnumerable<Module> modules =
                    (from m in await db.Modules.ToListAsync()
                     join om in ongoingModules on m.Id equals om.ModuleId
                     select m).ToList();
                foreach (ModuleHistory ongoingModule in ongoingModules)
                {
                    new TimerAssociates().StartModuleTimer(ongoingModule.Id, TimeSpan.FromMinutes(modules.Where(m => m.Id == ongoingModule.ModuleId)
                        .Select(m => m.MinutesToPass).SingleOrDefault()));
                }
            }
        }

        public void DisposeTimer(int moduleHistoryId)
        {
            if (ModuleTimers.TryGetValue(moduleHistoryId, out TimerAssociates timerAssociatives))
            {
                timerAssociatives.timer.Dispose();
                ModuleTimers.Remove(moduleHistoryId);
            }
        }

        public int TimeLeft(int moduleHistoryId)
        {
            if (ModuleTimers.TryGetValue(moduleHistoryId, out TimerAssociates timerAssociatives))
            {
                return Convert.ToInt32((timerAssociatives.moduleFinish - DateTime.UtcNow).TotalMilliseconds);
            }
            return 0;
        }

        private async void StopModuleOnTimer(object state)
        {
            await new LectureHistoryHelper().ModulePassed((int)state);
        }
    }
}