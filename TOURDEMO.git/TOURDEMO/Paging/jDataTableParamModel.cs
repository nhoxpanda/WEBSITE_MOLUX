using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOURDEMO.Paging
{
    public class jDataTableParamModel
    {
        public jDataTableParamModel()
        {
            columns = new Dictionary<string, JDataTableColumn>();
            order = new List<JDataTableOrderType>();
        }
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public string search_value { get; set; }
        public bool search_regex { get; set; }

        public IDictionary<string,JDataTableColumn> columns { get; set; }

        public List<JDataTableOrderType> order { get; set; }

        
    }

    public class JDataTableOrderType
    {
        public int column { get; set; }
        public bool isDesc { get; set; }
    }

    public class JDataTableColumn
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public string search { get; set; }
        public bool regex { get; set; }
    }

    //public class ListPagingSelected : List<JDataTableColumn>
    //{
    //    public ListPagingSelected()
    //    {
    //        SelectedOrder = null;
    //        SelectedOrderIndex = -1;
    //    }
    //    public JDataTableColumn SelectedOrder { get; set; }
    //    public int SelectedOrderIndex { get; set; }
    //}
}
