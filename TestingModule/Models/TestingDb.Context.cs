﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class testingDbEntities : DbContext
    {
        public testingDbEntities()
            : base("name=testingDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<CummulativeRespons> CummulativeResponses { get; set; }
        public virtual DbSet<CumulativeTestsLecture> CumulativeTestsLectures { get; set; }
        public virtual DbSet<CumulativeTestsPassed> CumulativeTestsPasseds { get; set; }
        public virtual DbSet<Discipline> Disciplines { get; set; }
        public virtual DbSet<ExeptionLog> ExeptionLogs { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<IndividualRespons> IndividualResponses { get; set; }
        public virtual DbSet<IndividualTestsPassed> IndividualTestsPasseds { get; set; }
        public virtual DbSet<LectorDiscipline> LectorDisciplines { get; set; }
        public virtual DbSet<Lector> Lectors { get; set; }
        public virtual DbSet<LectureHistoryGroup> LectureHistoryGroups { get; set; }
        public virtual DbSet<Lecture> Lectures { get; set; }
        public virtual DbSet<LecturesHistory> LecturesHistories { get; set; }
        public virtual DbSet<ModuleHistory> ModuleHistories { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<RealtimeModulesPassed> RealtimeModulesPasseds { get; set; }
        public virtual DbSet<RealtimeRespons> RealtimeResponses { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Speciality> Specialities { get; set; }
        public virtual DbSet<StudentDiscipline> StudentDisciplines { get; set; }
        public virtual DbSet<Student> Students { get; set; }
    }
}
