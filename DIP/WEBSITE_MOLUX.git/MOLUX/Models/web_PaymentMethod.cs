//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MOLUX.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class web_PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public Nullable<int> Orders { get; set; }
        public string URL { get; set; }
    }
}
