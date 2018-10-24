using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using TestingModule.Additional;
using TestingModule.Hubs;

namespace TestingModule.Models
{
    public class TimerAssociates : IDisposable
    {
        private readonly testingDbEntities _context;
        private Timer _timer;
        public DateTime ModuleFinish;
        private static readonly Dictionary<int, TimerAssociates> ModuleTimers = new Dictionary<int, TimerAssociates>();
        private static readonly Dictionary<int, TimerAssociates> IndividualQuizTimers = new Dictionary<int, TimerAssociates>();
        private static readonly Dictionary<int, TimerAssociates> CumulativeQuizTimers = new Dictionary<int, TimerAssociates>();

        /*public TimerAssociates(Timer timer, DateTime moduleFinish, testingDbEntities context)
        {
            _context = context;
            this.timer = timer;
            ModuleFinish = moduleFinish;
        }*/

        private void CreateTimer(Timer timer, DateTime moduleFinish)
        {
            _timer = timer;
            ModuleFinish = moduleFinish;
        }

        public TimerAssociates(testingDbEntities context)
        {
            _context = context;
        }

        public struct TimerType
        {
            public const int RealtimeId = 1;
            public const int IndividualId = 2;
            public const int CumulativeId = 3;
        }

        public static TimerAssociates GetTimer(int historyId, int timerType)
        {
            switch (timerType)
            {
                case TimerType.RealtimeId:
                    if (ModuleTimers.TryGetValue(historyId, out TimerAssociates realtimeTimer))
                        return realtimeTimer;
                    break;
                case TimerType.IndividualId:
                    if (IndividualQuizTimers.TryGetValue(historyId, out TimerAssociates individualTimer))
                        return individualTimer;
                    break;
                case TimerType.CumulativeId:
                    if (CumulativeQuizTimers.TryGetValue(historyId, out TimerAssociates cumulativeTimer))
                        return cumulativeTimer;
                    break;
            }
            return null;
        }

        public void StartTimer(int historyId, TimeSpan timeToPass, int timerType)
        {
            switch (timerType)
            {
                case TimerType.RealtimeId:
                    CreateTimer(new Timer(StopQuizOnTimer, Tuple.Create(historyId, timerType), timeToPass, TimeSpan.Zero), DateTime.UtcNow + timeToPass);
                    ModuleTimers.Add(historyId, this);
                    break;
                case TimerType.IndividualId:
                    if (!IndividualQuizTimers.ContainsKey(historyId))
                    {
                        CreateTimer(new Timer(StopQuizOnTimer, Tuple.Create(historyId, timerType), timeToPass, TimeSpan.Zero), DateTime.UtcNow + timeToPass);
                        IndividualQuizTimers.Add(historyId, this);
                    }
                    break;
                case TimerType.CumulativeId:
                    if (!CumulativeQuizTimers.ContainsKey(historyId))
                    {
                        CreateTimer(new Timer(StopQuizOnTimer, Tuple.Create(historyId, timerType), timeToPass, TimeSpan.Zero), DateTime.UtcNow + timeToPass);
                        CumulativeQuizTimers.Add(historyId, this);
                    }
                    break;
            }
        }

        public async Task OnStartModuleTimer()
        {
            if (await _context.ModuleHistories.AnyAsync(mh => mh.StartTime != null && mh.IsPassed != true))
            {
                IEnumerable<ModuleHistory> ongoingModules =
                    await (from mh in _context.ModuleHistories
                           where mh.StartTime != null && mh.IsPassed != true
                           select mh).ToListAsync();
                IEnumerable<Module> modules =
                    (from m in await _context.Modules.ToListAsync()
                     join om in ongoingModules on m.Id equals om.ModuleId
                     select m).ToList();
                foreach (ModuleHistory ongoingModule in ongoingModules)
                {
                    TimerAssociates timerAssociates = new TimerAssociates(_context);
                    timerAssociates.StartTimer(ongoingModule.Id, TimeSpan.FromMinutes(modules.Where(m => m.Id == ongoingModule.ModuleId)
                    .Select(m => m.MinutesToPass).SingleOrDefault()), TimerType.RealtimeId);

                }
            }
        }

        public void DisposeTimer(int historyId, int timerType)
        {
            switch (timerType)
            {
                case TimerType.RealtimeId:
                    if (ModuleTimers.TryGetValue(historyId, out TimerAssociates realtimeTimerAssociates))
                    {
                        realtimeTimerAssociates._timer.Dispose();
                        ModuleTimers.Remove(historyId);
                    }
                    break;
                case TimerType.IndividualId:
                    if (IndividualQuizTimers.TryGetValue(historyId, out TimerAssociates indivdualTimerAssociates))
                    {
                        indivdualTimerAssociates._timer.Dispose();
                        IndividualQuizTimers.Remove(historyId);
                    }
                    break;
                case TimerType.CumulativeId:
                    if (CumulativeQuizTimers.TryGetValue(historyId, out TimerAssociates cumulativeTimerAssociates))
                    {
                        cumulativeTimerAssociates._timer.Dispose();
                        CumulativeQuizTimers.Remove(historyId);
                    }
                    break;

            }

        }

        public int TimeLeft(int historyId, int timerType)
        {
            switch (timerType)
            {
                case TimerType.RealtimeId:
                    if (ModuleTimers.TryGetValue(historyId, out TimerAssociates realtimeTimerAssociates))
                    {
                        return Convert.ToInt32((realtimeTimerAssociates.ModuleFinish - DateTime.UtcNow).TotalSeconds);
                    }
                    return 0;
                case TimerType.IndividualId:
                    if (IndividualQuizTimers.TryGetValue(historyId, out TimerAssociates indivdualTimerAssociates))
                    {
                        return Convert.ToInt32((indivdualTimerAssociates.ModuleFinish - DateTime.UtcNow).TotalMilliseconds);
                    }
                    else
                    {
                        int timeLeft = Convert.ToInt32(Math.Round(TimeSpan.FromMinutes((from q in _context.Questions
                                                                                        join iq in _context.IndividualQuizPasseds on q.LectureId equals iq.LectureId
                                                                                        where iq.Id == historyId
                                                                                        select q).Count()).TotalMilliseconds / 2));
                        return timeLeft;
                    }
                case TimerType.CumulativeId:
                    if (CumulativeQuizTimers.TryGetValue(historyId, out TimerAssociates cumulativeTimerAssociates))
                    {
                        return Convert.ToInt32((cumulativeTimerAssociates.ModuleFinish - DateTime.UtcNow).TotalMilliseconds);
                    }
                    else
                    {
                        int timeLeft = Convert.ToInt32(Math.Round(TimeSpan.FromMinutes(20).TotalMilliseconds / 2));
                        return timeLeft;
                    }
            }
            return 0;
        }

        private async void StopQuizOnTimer(object state)
        {
            var tuple = (Tuple<int, int>)state;
            switch (tuple.Item2)
            {
                case TimerType.RealtimeId:
                    QuizHub.StopModule(tuple.Item1);
                    await new LectureHistoryHelper(_context).ModulePassed(tuple.Item1);
                    break;
                case TimerType.IndividualId:
                    await new QuizManager(_context).ResovlePassedIndividualQuiz(tuple.Item1);
                    break;
                case TimerType.CumulativeId:
                    await new QuizManager(_context).ResovlePassedCumulativeQuiz(tuple.Item1);
                    break;
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
            _timer?.Dispose();
        }
    }
}