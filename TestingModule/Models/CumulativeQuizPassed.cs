//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestingModule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CumulativeQuizPassed
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CumulativeQuizPassed()
        {
            this.CumulativeQuizLectures = new HashSet<CumulativeQuizLecture>();
            this.CumulativeResponses = new HashSet<CumulativeRespons>();
        }
    
        public int Id { get; set; }
        public int DisciplineId { get; set; }
        public int StudentId { get; set; }
        public bool IsPassed { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CumulativeQuizLecture> CumulativeQuizLectures { get; set; }
        public virtual Discipline Discipline { get; set; }
        public virtual Student Student { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CumulativeRespons> CumulativeResponses { get; set; }
    }
}
