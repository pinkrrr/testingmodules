using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using TestingModule.Additional;

namespace TestingModule.Models
{
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

        public static void StartTimer(int historyId, TimeSpan timeToPass, int timerType)
        {
            switch (timerType)
            {
                case TimerType.RealtimeId:
                    TimerAssociates realtimeTimerAssociates = new TimerAssociates(new Timer(StopQuizOnTimer, Tuple.Create(historyId, timerType), timeToPass, TimeSpan.Zero), DateTime.UtcNow + timeToPass);
                    ModuleTimers.Add(historyId, realtimeTimerAssociates);
                    break;
                case TimerType.IndividualId:
                    if (!IndividualQuizTimers.ContainsKey(historyId))
                    {
                        TimerAssociates indivdualTimerAssociates = new TimerAssociates(new Timer(StopQuizOnTimer, Tuple.Create(historyId, timerType), timeToPass, TimeSpan.Zero), DateTime.UtcNow + timeToPass);
                        IndividualQuizTimers.Add(historyId, indivdualTimerAssociates);
                    }
                    break;
                case TimerType.CumulativeId:
                    if (!CumulativeQuizTimers.ContainsKey(historyId))
                    {
                        TimerAssociates cumulativeTimerAssociates = new TimerAssociates(new Timer(StopQuizOnTimer, Tuple.Create(historyId, timerType), timeToPass, TimeSpan.Zero), DateTime.UtcNow + timeToPass);
                        CumulativeQuizTimers.Add(historyId, cumulativeTimerAssociates);
                    }
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
            db.Value.Dispose();
        }

        public static void DisposeTimer(int historyId, int timerType)
        {
            switch (timerType)
            {
                case TimerType.RealtimeId:
                    if (ModuleTimers.TryGetValue(historyId, out TimerAssociates realtimeTimerAssociates))
                    {
                        realtimeTimerAssociates.timer.Dispose();
                        ModuleTimers.Remove(historyId);
                    }
                    break;
                case TimerType.IndividualId:
                    if (IndividualQuizTimers.TryGetValue(historyId, out TimerAssociates indivdualTimerAssociates))
                    {
                        indivdualTimerAssociates.timer.Dispose();
                        IndividualQuizTimers.Remove(historyId);
                    }
                    break;
                case TimerType.CumulativeId:
                    if (CumulativeQuizTimers.TryGetValue(historyId, out TimerAssociates cumulativeTimerAssociates))
                    {
                        cumulativeTimerAssociates.timer.Dispose();
                        CumulativeQuizTimers.Remove(historyId);
                    }
                    break;

            }

        }

        public static int TimeLeft(int historyId, int timerType)
        {
            switch (timerType)
            {
                case TimerType.RealtimeId:
                    if (ModuleTimers.TryGetValue(historyId, out TimerAssociates realtimeTimerAssociates))
                    {
                        return Convert.ToInt32((realtimeTimerAssociates.moduleFinish - DateTime.UtcNow).TotalMilliseconds);
                    }
                    return 0;
                case TimerType.IndividualId:
                    if (IndividualQuizTimers.TryGetValue(historyId, out TimerAssociates indivdualTimerAssociates))
                    {
                        return Convert.ToInt32((indivdualTimerAssociates.moduleFinish - DateTime.UtcNow).TotalMilliseconds);
                    }
                    else
                    {
                        testingDbEntities db = new testingDbEntities();
                        int timeLeft = Convert.ToInt32(Math.Round(TimeSpan.FromMinutes((from q in db.Questions
                                                                           join iq in db.IndividualQuizPasseds on q.LectureId equals iq.LectureId
                                                                           where iq.Id == historyId
                                                                           select q).Count()).TotalMilliseconds / 2));
                        db.Dispose();
                        return timeLeft;
                    }
                case TimerType.CumulativeId:
                    if (CumulativeQuizTimers.TryGetValue(historyId, out TimerAssociates cumulativeTimerAssociates))
                    {
                        return Convert.ToInt32((cumulativeTimerAssociates.moduleFinish - DateTime.UtcNow).TotalMilliseconds);
                    }
                    else
                    {
                        testingDbEntities db = new testingDbEntities();
                        int timeLeft = Convert.ToInt32(Math.Round(TimeSpan.FromMinutes(20).TotalMilliseconds / 2));
                        db.Dispose();
                        return timeLeft;
                    }
            }
            return 0;
        }

        private static async void StopQuizOnTimer(object state)
        {
            var tuple = (Tuple<int, int>)state;
            switch (tuple.Item2)
            {
                case TimerType.RealtimeId:
                    await new LectureHistoryHelper().ModulePassed(tuple.Item1);
                    break;
                case TimerType.IndividualId:
                    await new QuizManager().ResovlePassedIndividualQuiz(tuple.Item1);
                    break;
                case TimerType.CumulativeId:
                    await new QuizManager().ResovlePassedCumulativeQuiz(tuple.Item1);
                    break;
            }
        }
    }
}