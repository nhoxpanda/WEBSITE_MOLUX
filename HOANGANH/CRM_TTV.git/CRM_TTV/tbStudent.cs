//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CRM_TTV
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbStudent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbStudent()
        {
            this.tbStudentFileAttaches = new HashSet<tbStudentFileAttach>();
        }
    
        public int idStudent { get; set; }
        public int idUser { get; set; }
        public int idClass { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public Nullable<int> achievements { get; set; }
        public string code { get; set; }
        public Nullable<int> timeLess { get; set; }
        public string listDateabsent { get; set; }
        public Nullable<int> status { get; set; }
    
        public virtual tbCategory tbCategory { get; set; }
        public virtual tbClass tbClass { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbStudentFileAttach> tbStudentFileAttaches { get; set; }
        public virtual tbUser tbUser { get; set; }
    }
}