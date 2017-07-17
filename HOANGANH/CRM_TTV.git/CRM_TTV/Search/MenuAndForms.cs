using System;
using System.Linq;
using System.Linq.Dynamic;

namespace CRM_TTV.Search
{
    public class MenuAndFormsSearchModel
    {
        //public int? PriceTo { get; set; }
        public string NameParent { get; set; }
        public string Name { get; set; }
        public string icon { get; set; }
        public string redirect { get; set; }
        public int? sort { get; set; }
        public bool isForm { get; set; }
    }
    public class MenuAndForms
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();
        public IQueryable<tbMenuAndForm> Filter(MenuAndFormsSearchModel searchModel, string order, Int32? size = 10, Int32? page = 0)
        {
            var result = db.tbMenuAndForms.AsQueryable();
            if (searchModel != null)
            {
                //if (searchModel.id.HasValue)
                //    result = result.Where(x => x.Id == searchModel.Id);
                if (!string.IsNullOrEmpty(searchModel.Name))
                    result = result.Where(x => x.name.Contains(searchModel.Name));
                if (!string.IsNullOrEmpty(searchModel.NameParent))
                    result = result.Where(x => x.name.Contains(searchModel.NameParent));
                if (!string.IsNullOrEmpty(searchModel.icon))
                    result = result.Where(x => x.icon.Contains(searchModel.icon));
                if (!string.IsNullOrEmpty(searchModel.redirect))
                    result = result.Where(x => x.redirect.Contains(searchModel.redirect));
                if (searchModel.sort.HasValue)
                    result = result.Where(x => x.menuID == searchModel.sort);
            }

            int take = (Int32)size;
            int skip = (Int32)page * take;

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order))
            {
                orderBy = order.Split('-');
                return result.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take);
            }

            return result.OrderBy(x => x.menuID).Skip(skip).Take(take);
        }
    }
}