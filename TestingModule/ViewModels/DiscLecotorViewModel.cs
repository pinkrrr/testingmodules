using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class DiscLecotorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? LectorId { get; set; }
        public IEnumerable<Lector> Lectors { get; set; }
    }
}