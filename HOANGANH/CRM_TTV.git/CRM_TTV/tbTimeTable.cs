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
    
    public partial class tbTimeTable
    {
        public int idTimetable { get; set; }
        public int idClass { get; set; }
        public int idSubject { get; set; }
        public System.DateTime date { get; set; }
        public System.TimeSpan startTime { get; set; }
        public System.TimeSpan endTime { get; set; }
        public string note { get; set; }
    
        public virtual tbClass tbClass { get; set; }
        public virtual tbSubject tbSubject { get; set; }
    }
}