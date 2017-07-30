using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class RegistrationFormViewModel
    {
        public Student Student { get; set; }
        public Account Account { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<Speciality> Specialities { get; set; }
        public IEnumerable<Role> Roles { get; set; }
    }
}