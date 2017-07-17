using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM_TTV.Models
{
    public class speaker
    {
        //[Range(0.01, 100.00, ErrorMessage = "Price must be between 0.01 and 100.00")]
        //[MaxLength(24), MinLength(5)]
        //[StringLength(160)]
        //[DisplayName("Artist")]
        //[Required(ErrorMessage = "Contact Name required!", AllowEmptyStrings = false)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public int type { get; set; } //1 sussess, 2 error, 3 alert
        public string title { get; set; }
        public string content { get; set; }
    }
}