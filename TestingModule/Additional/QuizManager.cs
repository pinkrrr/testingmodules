﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TestingModule.Models;
using System.Threading.Tasks;
using System.Web.DynamicData;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{
    public class QuizManager : IDisposable
    {
        private readonly testingDbEntities _context;
        private readonly StudentPageHelper _studentPageHelper;
        private readonly TimerAssociates _timerAssociates;

        public QuizManager(testingDbEntities context)
        {
            _context = context;
            _studentPageHelper = new StudentPageHelper(context);
            _timerAssociates = new TimerAssociates(context);
        }

        #region RealTimeQuiz Student

        private async Task<ICollection<Question>> GetQuestionsList(int moduleId)
        {
            ICollection<Question> questionsList = await _context.Questions.Where(q => q.ModuleId == moduleId)
                .ToListAsync();
            return questionsList;
        }

        private async Task<IEnumerable<Answer>> GetAnswersList(int questionId)
        {
            IEnumerable<Answer> answersList = await _context.Answers.Where(a => a.QuestionId == questionId)
                .ToListAsync();
            return answersList;
        }

        public async Task ResovlePassedRealtimeQuiz(int moduleId, int studentId, int moduleHistoryId, int lectureId)
        {
            _context.RealtimeModulesPasseds.Add(new RealtimeModulesPassed()
            {
                ModuleId = moduleId,
                StudentId = studentId,
                ModuleHistoryId = moduleHistoryId
            });
            var passedModules =
                await (from m in _context.Modules
                       where m.LectureId == lectureId
                       join rtmp in _context.RealtimeModulesPasseds on m.Id equals rtmp.ModuleId into groupjoin
                       from gj in groupjoin.DefaultIfEmpty()
                       where gj == null || gj.StudentId == studentId
                       select gj).ToListAsync();

            if (passedModules.All(a => a != null))
            {
                if (await _context.IndividualQuizPasseds.AnyAsync(iqp => iqp.LectureId == lectureId && iqp.StudentId == studentId))
                {
                    return;
                }
                IndividualQuizPassed individualTestsPassed = new IndividualQuizPassed()
                {
                    DisciplineId = _context.Lectures.Where(w => w.Id == lectureId).Select(s => s.DisciplineId)
                        .SingleOrDefault(),
                    IsPassed = false,
                    LectureId = lectureId,
                    StudentId = studentId
                };
                _context.IndividualQuizPasseds.Add(individualTestsPassed);
            }


            await _context.SaveChangesAsync();
        }


        public async Task<RealTimeQuizViewModel> GetRealtimeQnA(int moduleHistoryId)
        {
            var student = await AccountCredentials.GetStudent();
            bool studentCanPass = await _studentPageHelper.StudentCanPass(moduleHistoryId, student.Id);
            if (studentCanPass)
            {
                RealTimeQuizViewModel qnA = new RealTimeQuizViewModel();
                if (await _context.RealtimeModulesPasseds.AnyAsync(smp =>
                    smp.StudentId == student.Id && smp.ModuleHistoryId == moduleHistoryId))
                    return qnA;
                ModuleHistory moduleHistory =
                    await _context.ModuleHistories.SingleOrDefaultAsync(mh => mh.Id == moduleHistoryId);
                int lectureId = _context.LecturesHistories.Where(lh => lh.Id == moduleHistory.LectureHistoryId)
                    .Select(lh => lh.LectureId).SingleOrDefault();
                var question = await _context.Questions.Where(q => q.ModuleId == moduleHistory.ModuleId && q.QuestionType == QuestionType.RealtimeId &&
                                                                   !_context.RealtimeResponses.Where(r => r.ModuleHistoryId == moduleHistoryId &&
                                                                   r.StudentId == student.Id).Select(r => r.QuestionId).Contains(q.Id))
                                                                   .OrderBy(q => Guid.NewGuid()).FirstOrDefaultAsync();

                if (question == null)
                {
                    await ResovlePassedRealtimeQuiz(moduleHistory.ModuleId, student.Id, moduleHistoryId, lectureId);
                    return qnA;
                }

                qnA = new RealTimeQuizViewModel
                {
                    Question = question,
                    Student = student,
                    Answers = await GetAnswersList(question.Id),
                    LectureHistoryId = moduleHistory.LectureHistoryId,
                    ModuleHistoryId = moduleHistoryId
                };
                return qnA;
            }
            return null;
        }

        public async Task<RealTimeQuizViewModel> UpdateQuizModel(RealTimeQuizViewModel quizVM)
        {
            int moduleId = quizVM.Question.ModuleId;
            int lectureId = quizVM.Question.LectureId;
            quizVM.Question = await _context.Questions.Where(q => q.ModuleId == moduleId && q.QuestionType == QuestionType.RealtimeId &&
                                                                  !_context.RealtimeResponses.Where(r => r.ModuleHistoryId == quizVM.ModuleHistoryId &&
                                                                  r.StudentId == quizVM.Student.Id).Select(r => r.QuestionId).Contains(q.Id))
                                                                  .OrderBy(q => Guid.NewGuid()).FirstOrDefaultAsync();
            if (quizVM.Question == null)
            {
                await ResovlePassedRealtimeQuiz(moduleId, quizVM.Student.Id, quizVM.ModuleHistoryId, lectureId);
                return null;
            }
            quizVM.Answers = await GetAnswersList(quizVM.Question.Id);
            return quizVM;
        }

        #endregion

        #region IndividualQuiz

        public async Task<IndividualQuizViewModel> GetIndividualQnA(int individualQuizId)
        {
            Student student = await AccountCredentials.GetStudent();
            var question =
                await (from q in _context.Questions
                       where !_context.IndividualResponses.Any(ir => ir.IndividualQuizId == individualQuizId && ir.QuestionId == q.Id)// && q.QuestionType == QuestionType.IndividualId
                       join it in _context.IndividualQuizPasseds on q.LectureId equals it.LectureId
                       where it.Id == individualQuizId
                       select q).OrderBy(q => Guid.NewGuid()).FirstOrDefaultAsync();

            if (question == null)
            {
                await ResovlePassedIndividualQuiz(individualQuizId);
                return null;
            }

            return new IndividualQuizViewModel
            {
                Question = question,
                Student = student,
                Answers = await GetAnswersList(question.Id),
                IndividualQuizId = individualQuizId,
                TimeLeft = _timerAssociates.TimeLeft(individualQuizId, TimerAssociates.TimerType.Individual)
            };
        }

        public async Task ResovlePassedIndividualQuiz(int individualQuizId)
        {
            var toUpdate = await _context.IndividualQuizPasseds.SingleOrDefaultAsync(w => w.Id == individualQuizId);
            if (toUpdate == null)
            {
                return;
            }
            toUpdate.IsPassed = true;
            toUpdate.EndDate = DateTime.UtcNow;
            _timerAssociates.DisposeTimer(individualQuizId, TimerAssociates.TimerType.Individual);
            if (_context.IndividualQuizPasseds.Any(it => it.DisciplineId == toUpdate.DisciplineId && it.IsPassed && it.StudentId == toUpdate.StudentId))
            {
                var passedLectures =
                    await (from l in _context.Lectures
                           where l.DisciplineId == toUpdate.DisciplineId
                           join iqp in _context.IndividualQuizPasseds on l.Id equals iqp.LectureId into groupjoin
                           from gj in groupjoin.DefaultIfEmpty()
                           where gj == null || gj.StudentId == toUpdate.StudentId
                           select gj).ToListAsync();

                if (passedLectures.All(a => a != null))
                {
                    CumulativeQuizPassed cumulativeTestsPassed = new CumulativeQuizPassed()
                    {
                        DisciplineId = toUpdate.DisciplineId,
                        IsPassed = false,
                        StudentId = toUpdate.StudentId
                    };
                    var returnvalue = _context.CumulativeQuizPasseds.Add(cumulativeTestsPassed);
                    IEnumerable<CumulativeQuizLecture> cumulativeLectures =
                        from pl in passedLectures
                        where pl != null
                        select new CumulativeQuizLecture()
                        {
                            CumulativeQuizId = returnvalue.Id,
                            LectureId = pl.LectureId
                        };
                    _context.CumulativeQuizLectures.AddRange(cumulativeLectures);
                }
            }
            await _context.SaveChangesAsync();
        }

        #endregion

        #region CumulativeQuiz

        public async Task<CumulativeQuizViewModel> GetCumulativeQnA(int cumulativeQuizId)
        {
            Student student = await AccountCredentials.GetStudent();
            var question =
                await (from q in _context.Questions
                       where !_context.CumulativeResponses.Any(cr => cr.Id == cumulativeQuizId && cr.QuestionId == q.Id) // && q.QuestionType == QuestionType.IndividualId
                       group q by q.Id into groupjoin
                       from gj in groupjoin
                       join cql in _context.CumulativeQuizLectures on gj.LectureId equals cql.LectureId
                       where cql.CumulativeQuizId == cumulativeQuizId && _context.CumulativeResponses.Count(cr => cr.CumulativeQuizId == cql.CumulativeQuizId && cql.LectureId == gj.LectureId) <=
                             Math.Floor((double)(groupjoin.Count(c => c.LectureId == cql.LectureId) / groupjoin.Count() * 20))
                       select gj).OrderBy(q => Guid.NewGuid()).FirstOrDefaultAsync();

            if (question == null)
            {
                await ResovlePassedCumulativeQuiz(cumulativeQuizId);
                return null;
            }
            return new CumulativeQuizViewModel
            {
                Question = question,
                Student = student,
                Answers = await GetAnswersList(question.Id),
                CumulativeQuizId = cumulativeQuizId,
                TimeLeft = _timerAssociates.TimeLeft(cumulativeQuizId, TimerAssociates.TimerType.Cumulative)
            };
        }

        public async Task ResovlePassedCumulativeQuiz(int cumulativeQuizId)
        {
            var toUpdate = await _context.CumulativeQuizPasseds.SingleOrDefaultAsync(cqp => cqp.Id == cumulativeQuizId);
            if (toUpdate != null)
            {
                toUpdate.IsPassed = true;
                toUpdate.EndDate = DateTime.UtcNow;
                _timerAssociates.DisposeTimer(cumulativeQuizId, TimerAssociates.TimerType.Individual);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Realtime Statistics

        public async Task<StatisticsViewModel> GetHistoriesForLector()
        {
            Lector lector = await AccountCredentials.GetLector();
            IEnumerable<Discipline> disciplines =
                await (from ld in _context.LectorDisciplines
                       join d in _context.Disciplines on ld.DisciplineId equals d.Id
                       where ld.LectorId == lector.Id
                       select d).ToListAsync();
            IEnumerable<Lecture> lectures =
                (from d in disciplines
                 join l in await _context.Lectures.ToListAsync() on d.Id equals l.DisciplineId
                 select l).ToList();
            IEnumerable<LecturesHistory> histories =
               (from l in lectures
                join h in await _context.LecturesHistories.ToListAsync() on l.Id equals h.LectureId
                select h).ToList();
            StatisticsViewModel totalStatistics = new StatisticsViewModel
            {
                Lector = lector,
                Disciplines = disciplines,
                Lectures = lectures,
                Histories = histories
            };
            return totalStatistics;
        }

        public async Task<ResponseStatisticsViewModel> GetModulesForLector(int lectureHistoryId)
        {
            IEnumerable<ResponseTable> tableResponses =
                from rs in _context.RealtimeResponses
                join lhg in _context.LectureHistoryGroups on rs.LectureHistoryId equals lhg.LectureHistoryId
                join qs in _context.Questions on rs.QuestionId equals qs.Id
                where rs.LectureHistoryId == lectureHistoryId && lhg.GroupId == rs.GroupId
                select new ResponseTable
                {
                    AnswerId = rs.AnswerId,
                    LectureHistoryId = rs.LectureHistoryId,
                    GroupId = lhg.GroupId,
                    ModuleId = qs.ModuleId,
                    QuestionId = rs.QuestionId,
                    StudentId = rs.StudentId,
                    ResponseId = rs.Id
                };

            IEnumerable<Group> groups =
                await (from g in _context.Groups
                       join tr in tableResponses on g.Id equals tr.GroupId
                       group g by g.Id into gj
                       select gj.FirstOrDefault()).ToListAsync();

            IEnumerable<Module> modules =
                from m in _context.Modules
                join tr in tableResponses on m.Id equals tr.ModuleId
                group m by m.Id into gj
                select gj.FirstOrDefault();

            IEnumerable<Question> questions =
                from q in _context.Questions
                join m in modules on q.ModuleId equals m.Id
                select q;

            IEnumerable<Answer> answers =
                 await (from a in _context.Answers
                        join q in questions on a.QuestionId equals q.Id
                        select a).ToListAsync();

            IEnumerable<RealtimeRespons> responses =
                await (from r in _context.RealtimeResponses
                       join tr in tableResponses on r.Id equals tr.ResponseId
                       select r).ToListAsync();

            ICollection<AnswersForGroup> answersCount = new List<AnswersForGroup>();
            foreach (var group in groups)
            {
                foreach (var answer in answers)
                {
                    answersCount.Add(new AnswersForGroup
                    {
                        GroupId = group.Id,
                        QuestionId = answer.QuestionId,
                        Count = responses.All(r => r.AnswerId != answer.Id) ? 0 : responses.Count(r => r.AnswerId == answer.Id && r.GroupId == group.Id),
                        Text = answers.Where(a => a.Id == answer.Id).Select(a => a.Text).SingleOrDefault()
                    });
                }
            }

            ResponseStatisticsViewModel responseStatistics = new ResponseStatisticsViewModel
            {
                Modules = modules,
                Groups = groups,
                Questions = questions,
                AnswersCount = answersCount
            };
            return responseStatistics;
        }

        public async Task<RealTimeStatisticsViewModel> GetRealTimeStatisticsViewModel(Lector lector)
        {
            var realTimeStatistics =
                await (
                    from mh in _context.ModuleHistories
                    where mh.StartTime != null && mh.IsPassed == false && mh.LectorId == lector.Id
                    join lh in _context.LecturesHistories on mh.LectureHistoryId equals lh.Id
                    join m in _context.Modules on mh.ModuleId equals m.Id
                    join q in _context.Questions on m.Id equals q.ModuleId into qjoin
                    join sd in _context.StudentDisciplines on lh.DisciplineId equals sd.DisciplineId into sdjoin
                    select new RealTimeStatisticsViewModel
                    {
                        LecturesHistory = lh,
                        Module = m,
                        ModuleHistory = mh,
                        Questions = qjoin,
                        StudentIds = sdjoin.Select(s => s.StudentId)
                    }).SingleOrDefaultAsync();

            var timer = TimerAssociates.GetTimer(realTimeStatistics.ModuleHistory.Id, TimerAssociates.TimerType.Realtime) ??
                        _timerAssociates.StartTimer(realTimeStatistics.ModuleHistory.Id,
                            TimeSpan.FromMinutes(realTimeStatistics.Module.MinutesToPass), TimerAssociates.TimerType.Realtime);
            realTimeStatistics.Lector = lector;
            realTimeStatistics.TimeFinish = timer.ModuleFinish.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss");
            realTimeStatistics.Groups =
                await (from g in _context.Groups
                       join lhg in _context.LectureHistoryGroups on g.Id equals lhg.GroupId
                       where lhg.LectureHistoryId == realTimeStatistics.LecturesHistory.Id
                       select g).ToListAsync();

            return realTimeStatistics;
        }

        public IEnumerable<RealTimeStatistics> GetRealTimeStatistics(RealTimeStatisticsViewModel rtsVM)
        {
            IEnumerable<Answer> answers =
                (from a in _context.Answers.ToList()
                 join q in rtsVM.Questions on a.QuestionId equals q.Id
                 select a).ToList();
            IEnumerable<RealtimeRespons> responses =
                (from r in _context.RealtimeResponses.ToList()
                 where r.ModuleHistoryId == rtsVM.ModuleHistory.Id
                 select r).ToList();
            ICollection<RealTimeStatistics> realTimeStatistics = new List<RealTimeStatistics>();
            foreach (var group in rtsVM.Groups)
            {
                foreach (var question in rtsVM.Questions)
                {
                    realTimeStatistics.Add(new RealTimeStatistics
                    {
                        GroupId = group.Id,
                        QuestionId = question.Id,
                        TotalAnswers = !responses.Any(r => r.QuestionId == question.Id && r.GroupId == group.Id) ? 0 : responses.Count(r => r.QuestionId == question.Id && r.GroupId == group.Id),
                        CorrectAnswers = !responses.Any(r => r.QuestionId == question.Id && r.GroupId == group.Id && answers.Where(a => a.Id == r.AnswerId).Select(a => a.IsCorrect).SingleOrDefault() == true) ? 0
                        : responses.Count(r => r.QuestionId == question.Id && r.GroupId == group.Id && answers.Where(a => a.Id == r.AnswerId).Select(a => a.IsCorrect).SingleOrDefault() == true)
                    });
                }
            }
            return realTimeStatistics;
        }

        public async Task<Dictionary<int, int>> GetQuizForLectureAlailability(int disciplineId)
        {
            var studentId = AccountCredentials.GetStudentId();
            return await (from itp in _context.IndividualQuizPasseds
                          where !itp.IsPassed && itp.StudentId == studentId && itp.DisciplineId == disciplineId
                          select new { itp.LectureId, itp.Id }).ToDictionaryAsync(td => td.LectureId, td => td.Id);
        }

        #endregion


        public void Dispose()
        {
            _context?.Dispose();
            _studentPageHelper.Dispose();
            _timerAssociates.Dispose();
        }
    }
}