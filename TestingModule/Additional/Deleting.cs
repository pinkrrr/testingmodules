using System.Data.Entity;
using System.Linq;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class Deleting
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public void DeleteDiscipline(int disciplineId)
        {
            var disc = new Discipline() { Id = disciplineId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteLecture(int lectureId)
        {
            var lec = new Lecture() { Id = lectureId };
            _db.Entry(lec).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteModule(int moduleId)
        {
            var mod = new Module() { Id = moduleId };
            _db.Entry(mod).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteSpeciality(int specialityId)
        {
            var spc = new Speciality() { Id = specialityId };
            _db.Entry(spc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteGroup(int groupId)
        {
            var grp = new Group() { Id = groupId };
            _db.Entry(grp).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteStudent(int studentId)
        {
            var std = new Student() { Id = studentId };
            var ac = new Account() { Id = _db.Students.Where(t => t.Id == studentId).Select(t => t.AccountId).FirstOrDefault() };
            _db.Entry(std).State = EntityState.Deleted;
            _db.Entry(ac).State = EntityState.Deleted;
            _db.SaveChanges();
        }

    }
}