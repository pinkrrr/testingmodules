//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestingModule.Models
{
    using System;
    using System.Collections.Generic;

    public partial class StudentDiscipline
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int DisciplineId { get; set; }
        public bool IsSelected { get; set; }
    }
}
