using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestingModule.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public int SpecialityId { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int AccountId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}