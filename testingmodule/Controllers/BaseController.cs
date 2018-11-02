using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TestingModule.Additional;
using TestingModule.Models;

namespace TestingModule.Controllers
{
    public class BaseController : Controller
    {
        private readonly Lazy<testingDbEntities> _context;

        public BaseController(ITestingDbEntityService context)
        {
            _context = new Lazy<testingDbEntities>(context.GetContext);
        }

        protected testingDbEntities Context => _context.Value;

        protected string UserRole => ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role).Value;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}