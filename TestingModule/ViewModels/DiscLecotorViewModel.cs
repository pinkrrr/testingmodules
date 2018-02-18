using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestingModule.Models;

namespace TestingModule.ViewModels
{
    public class DiscLecotorViewModel
    {
        public int DiscId { get; set; }
        public string DiscName { get; set; }
        public int? LectorId { get; set; }
        public IEnumerable<Lector> Lectors { get; set; }
    }

    public class DiscLectorCumulativeCheckViewModel : DiscLecotorViewModel
    {
        public int? CumulativeQuizId { get; set; }
    }
}