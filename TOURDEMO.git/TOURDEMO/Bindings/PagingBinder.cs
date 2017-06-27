using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Paging;

namespace TOURDEMO.Bindings
{
    public class PagingBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var objectModel = base.BindModel(controllerContext, bindingContext);
            if (bindingContext.ModelType == typeof(jDataTableParamModel) || bindingContext.ModelType.IsSubclassOf(typeof(jDataTableParamModel)))
            {
                HttpRequestBase request = controllerContext.HttpContext.Request;
                jDataTableParamModel model = objectModel as jDataTableParamModel;

                SetModelFromRequest(model, request);

                return model;
            }
            else
            {
                return objectModel;
            }
        }

        private void SetModelFromRequest(jDataTableParamModel model, HttpRequestBase request)
        {
            int value = 0;
            int.TryParse(request.Form.Get("draw"), out value);
            model.draw = value;

            int.TryParse(request.Form.Get("start"), out value);
            model.start = value;

            int.TryParse(request.Form.Get("length"), out value);
            model.length = value;

            model.search_value = request.Form.Get("search[value]");
            bool bValue = false;
            bool.TryParse(request.Form.Get("search[regex]"), out bValue);
            model.search_regex = bValue;

            model.columns = GetListColumnsPagingModel(request);
            model.order = GetListOrderTypePagingModel(request);
        }

        private List<JDataTableOrderType> GetListOrderTypePagingModel(HttpRequestBase request)
        {
            List<JDataTableOrderType> lstOrder = new List<JDataTableOrderType>();

            int i = 0;
            while (!string.IsNullOrEmpty(request.Form.Get("order[" + i + "][column]")))
            {
                int nValue = 0;
                int.TryParse(request.Form.Get("order[" + i + "][column]"), out nValue);
                var orderType = new JDataTableOrderType
                {
                    column = nValue,
                    isDesc = request.Form.Get("order[" + i + "][dir]") == "desc"
                };
                lstOrder.Add(orderType);
                i++;
            }
            return lstOrder;
        }


        private IDictionary<string, JDataTableColumn> GetListColumnsPagingModel(HttpRequestBase request)
        {
            IDictionary<string, JDataTableColumn> lstColumn = new Dictionary<string, JDataTableColumn>();

            int i = 0;
            while (!string.IsNullOrEmpty(request.Form.Get("columns[" + i + "][data]")))
            {
                JDataTableColumn column = new JDataTableColumn();
                column.data = request.Form.Get("columns[" + i + "][data]");
                column.name = request.Form.Get("columns[" + i + "][name]");
                column.search = request.Form.Get("columns[" + i + "][search][value]");

                bool bValue = false;
                bool.TryParse(request.Form.Get("columns[" + i + "][searchable]"), out bValue);
                column.searchable = bValue;

                bool.TryParse(request.Form.Get("columns[" + i + "][orderable]"), out bValue);
                column.orderable = bValue;

                bool.TryParse(request.Form.Get("columns[" + i + "][search][regex]"), out bValue);
                column.regex = bValue;

                lstColumn[column.data] = column;
                i++;
            }
            return lstColumn;
        }

    }


}
