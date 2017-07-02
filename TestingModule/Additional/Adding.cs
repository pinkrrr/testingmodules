using System.Data.Entity;
using System.Linq;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class Adding
    {
        private readonly testingDbEntities _db = new testingDbEntities();
        public string AddNewDiscipline(string name)
        {
            string result;
            var matches = _db.Disciplines.Count(t => t.Name == name);
            if (matches == 0)
            {
                var last = _db.Disciplines.OrderByDescending(t => t.ID).Select(t => t.ID).FirstOrDefault() + 1;
                var disciplinesTable = _db.Set<Discipline>();
                disciplinesTable.Add(new Discipline() { ID = last, Name = name });
                _db.SaveChanges();
                result = "New skill has been successfully added to DB!";
            }
            else
            {
                result = "Current skill has been already created before! Try another one.";
            }
            return result;
        }

        public void DeleteDiscipline(int disciplineId)
        {
            var disc = new Discipline() { ID = disciplineId };
            _db.Entry(disc).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void EditDiscipline(int disciplineId, string name)
        {
            var disc = _db.Disciplines.Where(t => t.ID == disciplineId).FirstOrDefault();
            disc.Name = name;
            _db.SaveChanges();
        }
    }
}