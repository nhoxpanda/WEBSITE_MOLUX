using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOURDEMO.Paging
{
    public static class PagingExtensitions
    {
        public static JDataTableResult<TResult> To<TSource, TResult>(this JDataTableResult<TSource> source, Func<TSource, TResult> selector) 
            where TSource : class 
            where TResult : class
        {
            var data = source.data.Select(selector).ToList();
            return new JDataTableResult<TResult>(data, source.draw, source.recordsTotal, source.recordsTotal);
        }
    }
}
