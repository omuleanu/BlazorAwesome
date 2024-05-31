using Microsoft.AspNetCore.Components;
using Omu.BlazorAwesome.Models.Utils;
using System;
using System.Linq.Expressions;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Column filter utils
    /// </summary>
    public static class ColumnFilterUtil
    {
        /// <summary>
        /// Textbox filter
        /// </summary>
        public static Column<T> StringFilter<T>(this Column<T> column, IGridStateProp<T> gopt, ComponentBase receiver)
        {
            column.Filter(new()
            {
                Render = col => gopt.State.FilterTextbox(receiver, col),
                Query = ColumnFilterUtil.StringQuery(gopt, column.Bind)
            });

            return column;
        }

        /// <summary>
        /// DateOp filter
        /// </summary>
        public static Column<T> DateOpFilter<T>(this Column<T> column, IGridStateProp<T> gopt, ComponentBase receiver)
        {
            column.Filter(new()
            {
                Render = col => gopt.State.FilterDatePickerOp(receiver, col.Column),
                Query = ColumnFilterUtil.OpQuery<T, DateTime>(gopt, column.Bind)
            });

            return column;
        }

        /// <summary>
        /// NumericOp filter
        /// </summary>
        public static Column<T> NumericOpFilter<T>(this Column<T> column, IGridStateProp<T> gopt, ComponentBase receiver)
        {
            column.Filter(new()
            {
                Render = cs => gopt.State.FilterNumeric<T, int?>(receiver, cs, new() { Min = 0 }, useOp: true),
                Query = ColumnFilterUtil.OpQuery<T, int>(gopt, column.Bind)
            });

            return column;
        }

        /// <summary>
        /// Get string query expression for grid
        /// </summary>
        public static Func<object, Expression<Func<TModel, bool>>> StringQuery<TModel>(IGridStateProp<TModel> gopt, string propName)
        {
            return val => AweExprUtil.ContainsExpr<TModel>(val, propName);
        }

        /// <summary>
        /// Get Op Query expression for grid
        /// </summary>        
        public static Func<object, Expression<Func<TModel, bool>>> OpQuery<TModel, TValue>(IGridStateProp<TModel> gopt, string propName)
        {
            return val => OpQueryExpression<TModel, TValue>(gopt, val, propName);
        }

        private static Expression<Func<TModel, bool>> OpQueryExpression<TModel, TValue>(IGridStateProp<TModel> gopt, object val, string propName)
        {
            var pval = (TValue)val;
            var opKey = propName + "_Op";

            // set = as default operator
            var opValue = gopt.State.FilterValues.ContainsKey(opKey) ? gopt.State.FilterValues[opKey].ToString() : "=";

            var itmParam = Expression.Parameter(typeof(TModel));
                        
            var valPropExpr = AweExprUtil.GetBindMemberExpression<TModel>(itmParam, propName);
            var valConstExpr = Expression.Constant(pval, typeof(TValue));

            if (opValue == "<")
            {
                return Expression.Lambda<Func<TModel, bool>>(Expression.LessThan(valPropExpr, valConstExpr), itmParam);
            }

            if (opValue == "<=")
            {
                return Expression.Lambda<Func<TModel, bool>>(Expression.LessThanOrEqual(valPropExpr, valConstExpr), itmParam);
            }

            if (opValue == ">")
            {
                return Expression.Lambda<Func<TModel, bool>>(Expression.GreaterThan(valPropExpr, valConstExpr), itmParam);
            }

            if (opValue == ">=")
            {
                return Expression.Lambda<Func<TModel, bool>>(Expression.GreaterThanOrEqual(valPropExpr, valConstExpr), itmParam);
            }

            if (opValue == "!=")
            {
                return Expression.Lambda<Func<TModel, bool>>(Expression.NotEqual(valPropExpr, valConstExpr), itmParam);
            }

            if (opValue == "=")
            {
                return Expression.Lambda<Func<TModel, bool>>(Expression.Equal(valPropExpr, valConstExpr), itmParam);
            }
            
            throw new ArgumentException($"uknown filter operator [{opValue}]");
        }
    }
}