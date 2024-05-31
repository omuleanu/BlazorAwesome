using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Omu.BlazorAwesome.Core
{
    /// <summary>
    /// Dynamic linq
    /// </summary>
    public static class Dlinq
    {
        /// <summary>
        /// generate OrderBy query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="orderRules"></param>
        /// <returns></returns>
        public static IQueryable<T> OrderBy<T>(IQueryable<T> source, IDictionary<string, Sort> orderRules)
        {
            return OrderBy(source, orderRules, null);
        }

        /// <summary>
        /// generate OrderBy query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="orderRules"></param>
        /// <param name="selectors"></param>
        /// <returns></returns>
        public static IQueryable<T> OrderBy<T>(IQueryable<T> source, IDictionary<string, Sort> orderRules, IDictionary<string, LambdaExpression> selectors)
        {
            var queryExpr = source.Expression;
            var methodAsc = "OrderBy";
            var methodDesc = "OrderByDescending";

            foreach (var rule in orderRules)
            {
                var props = rule.Key.Split(',');
                var isAsc =  rule.Value == Sort.Asc;
                foreach (var prop in props)
                {
                    Type resType = null;

                    LambdaExpression defSel = null;

                    if (selectors != null && selectors.ContainsKey(prop))
                    {
                        defSel = selectors[prop];
                        resType = defSel.ReturnType;
                    }

                    var selector = defSel ?? ParseSelector<T>(prop, out resType);

                    queryExpr = Expression.Call(typeof(Queryable), isAsc ? methodAsc : methodDesc, new[] { source.ElementType, resType }, queryExpr, Expression.Quote(selector));
                    methodAsc = "ThenBy";
                    methodDesc = "ThenByDescending";
                }
            }

            return source.Provider.CreateQuery<T>(queryExpr);
        }

        private static LambdaExpression ParseSelector<T>(string prop, out Type endPropType)
        {
            var o = Expression.Parameter(typeof(T), "o");

            PropertyInfo property;
            Expression propertyAccess;
            if (prop.Contains('.'))
            {
                var properties = prop.Split('.');
                property = typeof(T).GetProperty(properties[0]);

                CheckProperty<T>(property, properties[0]);

                propertyAccess = Expression.MakeMemberAccess(o, property);

                for (var i = 1; i < properties.Length; i++)
                {
                    property = property.PropertyType.GetProperty(properties[i]);
                    CheckProperty<T>(property, properties[i]);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = typeof(T).GetProperty(prop);
                CheckProperty<T>(property, prop);

                propertyAccess = Expression.MakeMemberAccess(o, property);
            }

            endPropType = property.PropertyType;
            return Expression.Lambda(propertyAccess, o);
        }

        private static void CheckProperty<T>(PropertyInfo property, string prop)
        {
            if (property == null)
            {
                throw new AwesomeException(string.Format("Could not find property [{0}] on type [{1}]. Make sure you specify Key and Column.Bind correctly.", prop, typeof(T).Name));
            }
        }
    }
}