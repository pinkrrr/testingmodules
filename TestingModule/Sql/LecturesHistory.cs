//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestingModule.Sql
{
    using System;
    using System.Collections.Generic;
    
    public partial class LecturesHistory
    {
        public int ID { get; set; }
        public int LectureId { get; set; }
        public int DisciplineId { get; set; }
        public int GroupId { get; set; }
        public System.DateTime StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> ModulesPassed { get; set; }
    }
}
