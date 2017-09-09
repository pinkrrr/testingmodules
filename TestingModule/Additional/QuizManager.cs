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
            ICollection<Question> questions = new List<Question>();
            QuizViewModel qnA = new QuizViewModel { };
            if (answeredQuestions != null)
            {
                questions = await GetQuestionsList(moduleId);
                questions = questions.Where(t => !answeredQuestions.Contains(t.Id)).ToList();
                var question = questions.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                qnA = new QuizViewModel
                {
                    QuestionsList = questions,
                    Question = question,
                    Student = student,
                    Answers = await GetAnswersList(question.Id)
                };
            }
            //ICollection<Question> questions = await GetQuestionsList(moduleId);

            return qnA;
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
                from ld in _context.LectorDisciplines
                join d in _context.Disciplines on ld.DisciplineId equals d.Id
                where ld.LectorId == lector.Id
                select d;
            IEnumerable<Lecture> lectures =
                from d in disciplines
                join l in _context.Lectures on d.Id equals l.DisciplineId
                select l;
            IEnumerable<LecturesHistory> histories =
               from l in lectures
               join h in _context.LecturesHistories on l.Id equals h.LectureId
               select h;
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
            Lector lector = await new AccountCredentials().GetLector();
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
            IEnumerable<Group> groups = _context.Groups
                .Join(tableResponses, gr => gr.Id, tr => tr.GroupId,
                    (gr, tr) => new { Group = gr }).Select(gr => gr.Group).Distinct();
            IEnumerable<Module> modules = _context.Modules
                .Join(tableResponses, md => md.Id, tr => tr.ModuleId,
                    (md, tr) => new { Module = md }).Select(md => md.Module).Distinct();
            IEnumerable<Question> questions =
                from q in _context.Questions
                join m in modules on q.ModuleId equals m.Id
                select q;
            IEnumerable<Answer> answers =
                from a in _context.Answers
                join q in questions on a.QuestionId equals q.Id
                select a;
            IEnumerable<Respons> responses =
                from r in _context.Respons
                join tr in tableResponses on r.Id equals tr.ResponseId
                select r;
            ICollection<AnswersForGroup> answersCount = new List<AnswersForGroup>();
            foreach (var answer in answers)
            {
                int tempAnswerCount = 0;
                foreach (var group in groups)
                {
                    foreach (var response in responses.Where(r => r.AnswerId == answer.Id && r.GroupId == group.Id))
                    {
                        tempAnswerCount++;
                    }
                    answersCount.Add(new AnswersForGroup()
                    {
                        GroupId = group.Id,
                        Text = answer.Text,
                        Count = tempAnswerCount,
                        QuestionId = answer.QuestionId
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

}
}