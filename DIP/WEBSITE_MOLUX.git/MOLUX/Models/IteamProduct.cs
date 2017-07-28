using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOLUX.Models
{
    public class IteamProduct
    {
       public string MetaTitle { set;get;}
       public string MetaDescription { set; get; }
       public int RowID { set; get; }
       public string Name { set; get; }
       public string Picture { set; get; }
       public List<web_ItemImage> Images { set; get; }
       public string Code { set; get; }
       public string MadeIn { set; get; }
       public string Manufacturer_Code { set; get; }
       public string ShortDesc { set; get; }
       public string Status { set; get; }
       public Nullable<decimal> Sale_Price { set; get; }
       public string guarantee { set; get; }
       public string Description { set; get; }
       public List<web_ItemSizeColor> Sizes { set; get; }
       public List<web_ItemSizeColor> Colors { set; get; }
       public Nullable<decimal> Sale { set; get; }
       public string Item_Code { set; get; }
       public string Item_Code_2 { set; get; }
       public Nullable<DateTime> From_Date { set; get; }
       public Nullable<DateTime> To_Date { set; get; }
    }
}