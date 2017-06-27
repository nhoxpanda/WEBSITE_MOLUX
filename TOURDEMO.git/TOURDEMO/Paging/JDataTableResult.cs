using System.Collections.Generic;
using System.Linq;

namespace TOURDEMO.Paging
{
    public class JDataTableResult<T> where T : class
    {
        public JDataTableResult(IList<T> list, jDataTableParamModel param, int total = 0)
        {
            draw = param.draw;
            recordsTotal = total == 0 ? list.Count : total;
            recordsFiltered = list.Count;
            data = list.Skip(param.start).Take(param.length).ToList();
        }

        public JDataTableResult(IQueryable<T> query, jDataTableParamModel param, int total = 0)
        {
            draw = param.draw;
            recordsFiltered = query.Count();
            recordsTotal = total == 0 ? recordsFiltered : total;
            data = query.Skip(param.start).Take(param.length).ToList();
        }

        public JDataTableResult(IEnumerable<T> source, int draw, int total = 0, int totalFilter = 0)
        {
            this.draw = draw;
            recordsFiltered = totalFilter;
            recordsTotal = total == 0 ? recordsFiltered : total;
            data = source.ToList();
        }

        public int draw { get; private set; }
        public int recordsTotal { get; private set; }
        public int recordsFiltered { get; private set; }
        public List<T> data { get; private set; }
    }
}
