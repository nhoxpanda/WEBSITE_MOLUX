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
    
    public partial class web_ItemSizeColor
    {
        public int Id { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<int> SizeColorId { get; set; }
        public Nullable<int> Type { get; set; }
        public string Note { get; set; }
        public bool IsDelete { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual web_SizeColor web_SizeColor { get; set; }
    }
}