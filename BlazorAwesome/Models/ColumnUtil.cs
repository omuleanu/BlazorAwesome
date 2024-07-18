using Microsoft.AspNetCore.Components;
using Omu.BlazorAwesome.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid column utils
    /// </summary>
    public static class ColumnUtil
    {
        /// <summary>
        /// will set Column.Bind using Expressions 
        /// ( example: m => m.FirstName, m => m.LastName will result in Bind = "FirstName,LastName")
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="exprs"></param>
        /// <returns></returns>
        public static Column<T> For<T>(this Column<T> column, params Expression<Func<T, object>>[] exprs)
        {
            var result = new List<string>();

            foreach (var expr in exprs)
            {
                result.Add(AweExprUtil.GetExpressionStr(expr));
            }

            column.Bind = string.Join(",", result);

            return column;
        }

        /// <summary>
        /// Set column filter
        /// </summary>
        public static Column<T> Filter<T>(this Column<T> column, GridFilter<T> filter)
        {
            return AddColumnOpt(column, ColumnOpt.Filter, filter);
        }

        /// <summary>
        /// Get column filter
        /// </summary>
        public static GridFilter<T> GetFilter<T>(this Column<T> column)
        {
            if (column.Opt == null || !column.Opt.ContainsKey(ColumnOpt.Filter)) return null;

            return (GridFilter<T>)column.Opt[ColumnOpt.Filter];
        }

        /// <summary>
        /// Set column editor
        /// </summary>
        public static Column<T> Editor<T>(this Column<T> column, Func<ColumnEditorContext<T>, RenderFragment> render, bool isAction = false)
        {
            return AddColumnOpt(column, ColumnOpt.Editor, new GridEditor<T>
            {
                Render = render,
                IsAction = isAction
            });
        }

        /// <summary>
        /// Get column editor
        /// </summary>
        public static GridEditor<T> GetEditor<T>(this Column<T> column)
        {
            if (column.Opt == null || !column.Opt.ContainsKey(ColumnOpt.Editor)) return null;

            return (GridEditor<T>)column.Opt[ColumnOpt.Editor];
        }

        private static Column<T> AddColumnOpt<T>(Column<T> column, ColumnOpt columnOpt, object val)
        {
            if (column.Opt == null)
            {
                column.Opt = new Dictionary<ColumnOpt, object>();
            }

            column.Opt.Add(columnOpt, val);

            return column;
        }
    }
}