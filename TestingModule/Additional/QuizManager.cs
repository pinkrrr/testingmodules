using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using TestingModule.Models;
using System.Threading.Tasks;
using Microsoft.Owin;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{
    public class QuizManager
    {
        private readonly testingDbEntities _context = new testingDbEntities();

        public async Task<ICollection<Question>> GetQuestionsList(int moduleId)
        {
            ICollection<Question> questionsList = await _context.Questions.Where(q => q.ModuleId == moduleId)
                .ToListAsync();
            return questionsList;
        }

        public async Task<IEnumerable<Answer>> GetAnswersList(int questionId)
        {
            IEnumerable<Answer> answersList = await _context.Answers.Where(a => a.QuestionId == questionId)
                .ToListAsync();
            return answersList;
        }

        public async Task<QuizViewModel> GetQnA(int moduleId)
        {
            var student = await new AccountCredentials().GetStudent();
            var answeredQuestions = new StudentPageHelper().CheckActiveQuiz(student.Id);
            QuizViewModel qnA = new QuizViewModel();
            if (answeredQuestions != null)
            {
                ICollection<Question> questions = await GetQuestionsList(moduleId);
                questions = questions.Where(t => !answeredQuestions.Contains(t.Id)).ToList();
                if (questions.Count != 0)
                {
                    var question = questions.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                    qnA = new QuizViewModel
                    {
                        QuestionsList = questions,
                        Question = question,
                        Student = student,
                        Answers = await GetAnswersList(question.Id)
                    };
                }
                return qnA;
            }
            //ICollection<Question> questions = await GetQuestionsList(moduleId);

            return null;
        }

        public async Task<QuizViewModel> UpdateQuizModel(QuizViewModel quizVM)
        {
            Question questionToRemove = quizVM.QuestionsList.SingleOrDefault(ql => ql.Id == quizVM.Question.Id);
            quizVM.QuestionsList.Remove(questionToRemove);
            quizVM.Question = quizVM.QuestionsList.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            quizVM.Answers = await GetAnswersList(quizVM.Question.Id);
            return quizVM;
        }

        public async Task<int> GetLectureHistoryId(QuizViewModel qVM)
        {
            var lect = await _context.LecturesHistories
                .Join(_context.LectureHistoryGroups, lh => lh.Id, lhg => lhg.LectureHistoryId,
                    (lh, lhg) => new { LecturesHistory = lh, LectureHistoryGroup = lhg })
                .Where(t => t.LecturesHistory.LectureId == qVM.Question.LectureId &&
                            t.LecturesHistory.EndTime == null && t.LectureHistoryGroup.GroupId == qVM.Student.GroupId)
                .Select(t => t.LecturesHistory.Id).FirstOrDefaultAsync();
            return lect;
        }

        public async Task<TotalStatisticsViewModel> GetHistorieForLector()
        {
            Lector lector = await new AccountCredentials().GetLector();
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
            TotalStatisticsViewModel totalStatistics = new TotalStatisticsViewModel
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
                from rs in _context.Respons
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

            IEnumerable<Respons> responses =
                await (from r in _context.Respons
                       join tr in tableResponses on r.Id equals tr.ResponseId
                       select r).ToListAsync();

            IEnumerable<AnswersForGroup> answersCount =
                from a in answers
                join r in responses on a.Id equals r.AnswerId into gj
                from g in groups
                from groupjoin in gj.DefaultIfEmpty()
                select new AnswersForGroup
                {
                    GroupId = g.Id,
                    Text = a.Text,
                    Count = groupjoin == null ? 0 : responses.Count(r => r.AnswerId == a.Id && g.Id == groupjoin.GroupId),
                    QuestionId = a.QuestionId
                };
            ResponseStatisticsViewModel responseStatistics = new ResponseStatisticsViewModel
            {
                Modules = modules,
                Groups = groups,
                Questions = questions,
                AnswersCount = answersCount
            };
            return responseStatistics;
        }

    }
}