using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TestingModule.Models;

using System.Threading.Tasks;
using TestingModule.ViewModels;

namespace TestingModule.Additional
{
    public class QuizManager
    {
        private testingDbEntities _context=new testingDbEntities();

        public async Task<IEnumerable<Question>> GetQuestionsList(int moduleId)
        {
            IEnumerable<Question> questionsList=await _context.Questions.Where(q => q.ModuleId == moduleId)
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
            IEnumerable<Question> questions= await GetQuestionsList(moduleId);
            var question = questions.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            QuizViewModel qnA = new QuizViewModel
            {
                Question = question,
                Student = await new AccountCredentials().GetStudent(),
                Answers = await GetAnswersList(question.Id),
                Response = null
            };
            return qnA;
        }

    }
}