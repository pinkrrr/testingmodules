using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestingModule.Models
{
    public class StudentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SpecialityId { get; set; }
        public int GroupId { get; set; }
        public int StudentId { get; set; }
        public string Surname { get; set; }
    }
}