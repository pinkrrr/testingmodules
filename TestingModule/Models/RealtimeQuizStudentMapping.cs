using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestingModule.Models
{
    public class RealtimeQuizStudentMapping
    {
        private readonly Dictionary<int, HashSet<int>> _students =
            new Dictionary<int, HashSet<int>>();

        public int Count => _students.Count;

        public void Add(int key, int studentId)
        {
            lock (_students)
            {
                if (!_students.TryGetValue(key, out HashSet<int> students))
                {
                    students = new HashSet<int>();
                    _students.Add(key, students);
                }

                lock (students)
                {
                    if (students.All(s => s != studentId))
                    {
                        students.Add(studentId);
                    }
                }
            }
        }

        public IEnumerable<int> GetConnections(int key)
        {
            lock (_students)
            {
                if (_students.TryGetValue(key, out HashSet<int> students))
                {
                    return students;
                }
            }
            return Enumerable.Empty<int>();
        }

        public void Remove(int key, int studentId)
        {
            lock (_students)
            {
                if (!_students.TryGetValue(key, out HashSet<int> students))
                {
                    return;
                }

                lock (students)
                {
                    students.Remove(studentId);

                    if (students.Count == 0)
                    {
                        _students.Remove(key);
                    }
                }
            }
        }

    }
}