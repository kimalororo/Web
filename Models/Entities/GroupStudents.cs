//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LB2and3.Models.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class GroupStudents
    {
        public System.Guid StudentId { get; set; }
        public System.Guid GroupId { get; set; }
        public string PayementType { get; set; }
    
        public virtual Groups Groups { get; set; }
        public virtual Students Students { get; set; }
    }
}
