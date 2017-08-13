using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestingModule.Models;
using TestingModule.ViewModels;
using TestingModule.Additional;
using System.Threading.Tasks;

namespace TestingModule.Controllers
{
    public class QuizController : Controller
    {
        private testingDbEntities _context=new testingDbEntities();

        // GET: Quiz
        [Route("{controller}/{action}/{moduleId}")]
        public async Task<ActionResult> Index(int moduleId)
        {
            QuizViewModel qvm = await new QuizManager().GetQnA(moduleId);
            return View(qvm);
        }
    }
}