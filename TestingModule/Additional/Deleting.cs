using System.Data.Entity;
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
            var disc = new Lecture() { Id =  lectureId};
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteModule(int moduleId)
        {
            var disc = new Module() { Id = moduleId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteSpeciality(int SpecialityId)
        {
            var disc = new Speciality() { Id = SpecialityId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteGroup(int GroupId)
        {
            var disc = new Group() { Id = GroupId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteStudent(int StudentId)
        {
            var disc = new Student() { Id = StudentId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }

    }
}