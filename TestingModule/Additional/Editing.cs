using System.Linq;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class Editing
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public void EditDiscipline(int disciplineId, string name)
        {
            var disc = _db.Disciplines.FirstOrDefault(t => t.Id == disciplineId);
            disc.Name = name;
            _db.SaveChanges();
        }

        public void EditSpeciality(int specialityId, string name)
        {
            var disc = _db.Specialities.FirstOrDefault(t => t.Id == specialityId);
            disc.Name = name;
            _db.SaveChanges();
        }
    }
}