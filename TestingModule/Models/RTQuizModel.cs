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
        private static readonly Dictionary<int, TimerAssociates> IndividualQuizTimers = new Dictionary<int, TimerAssociates>();
        private static readonly Dictionary<int, TimerAssociates> CumulativeQuizTimers = new Dictionary<int, TimerAssociates>();

        public TimerAssociates(Timer timer, DateTime moduleFinish)
        {
            this.timer = timer;
            this.moduleFinish = moduleFinish;
        }

        public TimerAssociates() { }

        public struct TimerType
        {
            public const int RealtimeId = 1;
            public const int IndividualId = 2;
            public const int CumulativeId = 3;
        }

        public void StartTimer(int historyId, TimeSpan minutesToPass, int timerType)
        {
            switch (timerType)
            {
                case TimerType.RealtimeId:
                    TimerAssociates realtimeTimerAssociates = new TimerAssociates(
                        new Timer(
                            StopQuizOnTimer,
                            Tuple.Create(historyId, timerType),
                            minutesToPass,
                            TimeSpan.Zero),
                        DateTime.UtcNow + minutesToPass);
                    ModuleTimers.Add(historyId, realtimeTimerAssociates);
                    break;
                case TimerType.IndividualId:
                    TimerAssociates indivdualTimerAssociates = new TimerAssociates(
                        new Timer(
                            StopQuizOnTimer, Tuple.Create(historyId, timerType), minutesToPass, TimeSpan.Zero), DateTime.UtcNow + minutesToPass);
                    ModuleTimers.Add(historyId, indivdualTimerAssociates);
                    break;
                case TimerType.CumulativeId:
                    TimerAssociates cumulativeTimerAssociates = new TimerAssociates(new Timer(StopQuizOnTimer, Tuple.Create(historyId, timerType), minutesToPass, TimeSpan.Zero), DateTime.UtcNow + minutesToPass);
                    ModuleTimers.Add(historyId, cumulativeTimerAssociates);
                    break;
            }
        }

        public async Task OnStartModuleTimer()
        {
            Lazy<testingDbEntities> db = new Lazy<testingDbEntities>();
            if (await db.Value.ModuleHistories.AnyAsync(mh => mh.StartTime != null && mh.IsPassed != true))
            {
                IEnumerable<ModuleHistory> ongoingModules =
                    await (from mh in db.Value.ModuleHistories
                           where mh.StartTime != null && mh.IsPassed != true
                           select mh).ToListAsync();
                IEnumerable<Module> modules =
                    (from m in await db.Value.Modules.ToListAsync()
                     join om in ongoingModules on m.Id equals om.ModuleId
                     select m).ToList();
                foreach (ModuleHistory ongoingModule in ongoingModules)
                {
                    StartTimer(ongoingModule.Id, TimeSpan.FromMinutes(modules.Where(m => m.Id == ongoingModule.ModuleId)
                        .Select(m => m.MinutesToPass).SingleOrDefault()), TimerType.RealtimeId);
                }
            }
        }

        public void DisposeTimer(int moduleHistoryId, int timerType)
        {
            switch (timerType)
            {
                case TimerType.RealtimeId:
                    if (ModuleTimers.TryGetValue(moduleHistoryId, out TimerAssociates realtimeTimerAssociates))
                    {
                        realtimeTimerAssociates.timer.Dispose();
                        ModuleTimers.Remove(moduleHistoryId);
                    }
                    break;
                case TimerType.IndividualId:
                    if (ModuleTimers.TryGetValue(moduleHistoryId, out TimerAssociates indivdualTimerAssociates))
                    {
                        indivdualTimerAssociates.timer.Dispose();
                        ModuleTimers.Remove(moduleHistoryId);
                    }
                    break;
                case TimerType.CumulativeId:
                    if (ModuleTimers.TryGetValue(moduleHistoryId, out TimerAssociates cumulativeTimerAssociates))
                    {
                        cumulativeTimerAssociates.timer.Dispose();
                        ModuleTimers.Remove(moduleHistoryId);
                    }
                    break;

            }

        }

        public int TimeLeft(int moduleHistoryId, int timerType)
        {
            switch (timerType)
            {
                case TimerType.RealtimeId:
                    if (ModuleTimers.TryGetValue(moduleHistoryId, out TimerAssociates realtimeTimerAssociates))
                    {
                        return Convert.ToInt32((realtimeTimerAssociates.moduleFinish - DateTime.UtcNow).TotalMilliseconds);
                    }
                    return 0;
                case TimerType.IndividualId:
                    if (ModuleTimers.TryGetValue(moduleHistoryId, out TimerAssociates indivdualTimerAssociates))
                    {
                        indivdualTimerAssociates.timer.Dispose();
                        ModuleTimers.Remove(moduleHistoryId);
                    }
                    return 0;
                case TimerType.CumulativeId:
                    if (ModuleTimers.TryGetValue(moduleHistoryId, out TimerAssociates cumulativeTimerAssociates))
                    {
                        cumulativeTimerAssociates.timer.Dispose();
                        ModuleTimers.Remove(moduleHistoryId);
                    }
                    return 0;
            }
            return 0;
        }

        private async void StopQuizOnTimer(object state)
        {
            var tuple = (Tuple<int, int>)state;
            switch (tuple.Item2)
            {
                case TimerType.RealtimeId:
                    await new LectureHistoryHelper().ModulePassed(tuple.Item1);
                    break;
                case TimerType.IndividualId:
                    
                    break;
                case TimerType.CumulativeId:

                    break;
            }
        }
    }
}