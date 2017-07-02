using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class Editing
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public void EditDiscipline(int disciplineId, string name)
        {
            var disc = _db.Disciplines.Where(t => t.ID == disciplineId).FirstOrDefault();
            disc.Name = name;
            _db.SaveChanges();
        }
    }
}