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
    
    public partial class web_NewsCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public web_NewsCategory()
        {
            this.web_News = new HashSet<web_News>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string UrlCustom { get; set; }
        public Nullable<int> Orders { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDesc { get; set; }
        public Nullable<bool> IsShow { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> Type { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<web_News> web_News { get; set; }
    }
}