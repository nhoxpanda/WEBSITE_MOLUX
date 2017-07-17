using System;
using System.Linq;
using System.Linq.Dynamic;

namespace CRM_TTV.Search
{
    public class CategorySearchModel
    {
        public int? idCategoryType { get; set; }
        public int? sort { get; set; }
        //public int? PriceTo { get; set; }
        public string Name { get; set; }
        public string note { get; set; }
    }
    public class CategorySearch
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();
        public IQueryable<tbCategory> Filter(CategorySearchModel searchModel, string order, Int32? size = 10, Int32? page = 0)
        {
            var result = db.tbCategories.Where(x => x.idCategoryType == searchModel.idCategoryType).AsQueryable();
            if (searchModel != null)
            {
                //if (searchModel.id.HasValue)
                //    result = result.Where(x => x.Id == searchModel.Id);
                if (!string.IsNullOrEmpty(searchModel.Name))
                    result = result.Where(x => x.name.Contains(searchModel.Name));
                if (!string.IsNullOrEmpty(searchModel.note))
                    result = result.Where(x => x.note.Contains(searchModel.note));
                if (searchModel.sort.HasValue)
                    result = result.Where(x => x.sort == searchModel.sort);
            }

            int take = (Int32)size;
            int skip = (Int32)page * take;

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order))
            {
                orderBy = order.Split('-');
                return result.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take);
            }

            return result.OrderBy(x => x.idCategory).Skip(skip).Take(take);
        }
    }
}