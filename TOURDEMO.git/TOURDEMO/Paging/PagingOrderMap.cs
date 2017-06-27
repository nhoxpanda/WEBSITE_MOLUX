using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TOURDEMO.Paging
{
    public class PagingOrderMap<T> where T : new()
    {
        private bool _isDesc;
        private readonly IDictionary<int, PropertyInfo> _properties = new Dictionary<int, PropertyInfo>();

        public PagingOrderMap(bool isDesc)
        {
            this._isDesc = isDesc;
        }

        public void Add(int index, Expression<Func<T, object>> getProperty)
        {
            if(getProperty.Body is MemberExpression)
            {
                _properties[index] = (PropertyInfo)((MemberExpression)getProperty.Body).Member;
            }
            else
            {
                var op = ((UnaryExpression)getProperty.Body).Operand;
                _properties[index] = (PropertyInfo)((MemberExpression)op).Member;
            }
            
        }

        public IQueryable<T> OrderBy(IEnumerable<T> querySource, int index)
        {
            PropertyInfo propertyInfo = null;
            if (_properties.ContainsKey(index))
            {
                propertyInfo = _properties[index];
            }
            else
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (propertyInfo == null) propertyInfo = property;
                    var attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute))
                        as KeyAttribute;

                    if (attribute != null)
                    {
                        propertyInfo = property;
                        break;
                    }
                }
            }

            if (propertyInfo == null)
                throw new InvalidOperationException();
            //var parameter = Expression.Parameter(typeof(T));
            //var propAsObject = Expression.Convert(propertyInfo, typeof(object));
            var tType = typeof(T);
            var funcType = typeof(Func<,>)
                    .MakeGenericType(tType, propertyInfo.PropertyType);

            var lambdaBuilder = typeof(Expression)
                    .GetMethods()
                    .First(x => x.Name == "Lambda" && x.ContainsGenericParameters && x.GetParameters().Length == 2)
                    .MakeGenericMethod(funcType);

            var parameter = Expression.Parameter(tType);
            var propExpress = Expression.Property(parameter, propertyInfo);

            var sortLambda = lambdaBuilder
                    .Invoke(null, new object[] { propExpress, new ParameterExpression[] { parameter } });

            var sorter = typeof(Queryable)
                    .GetMethods()
                    .FirstOrDefault(x => x.Name == (_isDesc ? "OrderByDescending" : "OrderBy") && x.GetParameters().Length == 2)
                    .MakeGenericMethod(new[] { tType, propertyInfo.PropertyType });

            return (IQueryable<T>)sorter.Invoke(null, new object[] { querySource, sortLambda });

            //if (!_isDesc)
            //{
            //    return querySource.OrderBy(propertyInfo.Name);
            //}
            //else
            //{
            //    return querySource.OrderByDescending(x => propertyInfo.GetValue(x));
            //}
        }
    }
}
