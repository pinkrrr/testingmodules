using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class Deleting
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public void DeleteDiscipline(int disciplineId)
        {
            var disc = new Discipline() { ID = disciplineId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteLecture(int lectureId)
        {
            var disc = new Lecture() { ID =  lectureId};
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteModule(int moduleId)
        {
            var disc = new Module() { ID = moduleId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
    }
}