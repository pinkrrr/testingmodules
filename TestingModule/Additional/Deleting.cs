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
        public void DeleteSpeciality(int SpecialityId)
        {
            var disc = new Speciality() { ID = SpecialityId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteGroup(int GroupId)
        {
            var disc = new Group() { ID = GroupId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteStudent(int StudentId)
        {
            var disc = new Student() { ID = StudentId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }

    }
}