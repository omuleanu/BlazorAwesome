using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;
using Omu.BlazorAwesome.Components;
using Omu.BlazorAwesome.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

[assembly: InternalsVisibleTo("Test,PublicKey=00240000048000009400000006020000002400005253413100040000010001005d6a081022fe1da44adf02b2ced413a4e91d8a30ac8d3e0ada31f4b3a1edcebde97c9d480bb35f1f5df24f9d4727a10c1402fe306f9437fb5b37b2c08b8542bd9a5dae4e656348e8c4c6e8f4cd19d8a1fae95973032ba56dda495f91be05255f396105b8dcc3c84653873c2a8cefbf8d2a0c63ce13209840b64499b6c9551ec7")]
namespace Omu.BlazorAwesome.Models.Utils
{
    internal static class CompUtil
    {
        public static async ValueTask TryDestroy(IJSRuntime JS, ElementReference field)
        {
            try
            {
                await JS.InvokeVoidAsync(AweJsDestroy(), field);
            }
            catch (JSDisconnectedException)
            {
                // Ignore
            }
        }

        /// <summary>
        /// Check if values present in data
        /// </summary>
        public static async Task CheckValues<TKey>(IEnumerable<TKey> values, IEnumerable<KeyContent> data, Func<IEnumerable<TKey>, Task> setValue)
        {
            if (values is null || !values.Any() || data is null) return;

            var selectedItems = getSelectedValsItems(values, data);

            if (values.Count() != selectedItems.Count())
            {
                var nval = selectedItems.Select(o => o.Key).Cast<TKey>();
                await setValue(nval);
            }
        }

        private static List<KeyContent> getSelectedValsItems<TKey>(IEnumerable<TKey> values, IEnumerable<KeyContent> data)
        {
            var selectedItems = new List<KeyContent>();
            foreach (var val in values)
            {
                var item = data.FirstOrDefault(kv => object.Equals(kv.Key, val));
                if (item is not null)
                {
                    selectedItems.Add(item);
                }
            }

            return selectedItems;
        }

        public static ColumnState<T>[] GetSortColumns<T>(GridState<T> gridState)
        {
            var cs = gridState.ColumnsStates;
            return cs.Where(o => o.Sort != Sort.None && o.Group)
                .GroupBy(o => o.Column.Bind) // distinct by Bind
                .Select(o => o.First())
                .OrderBy(o => o.Rank)
                .Union(cs.Where(o => o.Sort != Sort.None && !o.Group).OrderBy(o => o.Rank))
                .ToArray();
        }

        public static bool HasClearButton(EditorWithClearOpt opt, OContext ocontext)
        {
            if (opt != null && opt.ClearBtn.HasValue) return opt.ClearBtn.Value;

            if (ocontext != null && ocontext.Opt != null)
            {
                return ocontext.Opt.ClearBtn;
            }

            return false;
        }

        public static string AweJsDestroy()
        {
            return CompUtil.AweJs("destrElm");
        }

        public static string AweJs(string name)
        {
            return $"{Consts.Aweclib}.{name}";
        }

        public static bool GridNeedsGroupbar<T>(GridState<T> gs)
        {
            return gs.ColumnsStates.Where(col => col.Column.Bind is not null).Any(col => col.Column.Groupable.Value);
        }

        public static IList<Column<T>> SetColumnsDefaults<T>(GridOpt<T> gopt, IList<Column<T>> columns)
        {
            var cid = 0;
            var ids = new List<string>();
            foreach (var column in columns)
            {
                column.Id ??= "c" + cid++;

                if (ids.Contains(column.Id))
                {
                    column.Id = "c" + cid++ + column.Id;
                }

                ids.Add(column.Id);
            }

            foreach (var column in columns.Where(o => o.Bind is not null))
            {
                if (column.Groupable is null) column.Groupable = gopt.Groupable;
                if (column.Sortable is null) column.Sortable = gopt.Sortable;
                if (column.Reorderable is null) column.Reorderable = gopt.Reorderable;
                if (column.Resizable is null) column.Resizable = gopt.Resizable;

                if (column.Group is true && column.Sort is Sort.None)
                {
                    column.Sort = Sort.Asc;
                }
            }

            return columns;
        }

        public static bool AreInSameGroup<T>(GridState<T> gs, string path, T o, T oo)
        {
            if (!path.Contains(",")) return gs.GetColumnStrVal(path, o) == gs.GetColumnStrVal(path, oo);
            var props = path.Split(',');

            var result = true;

            foreach (var prop in props)
            {
                result = result && gs.GetColumnStrVal(prop, o) == gs.GetColumnStrVal(prop, oo);
            }

            return result;
        }

        //public static string GetStrColumnVal<T>(string propName, T item)
        //{
        //    var val = GetColumnValue(propName, item);
        //    return val == null ? string.Empty : string.Join(" ", val);
        //}

        public static int GetMinWidth<T>(Column<T> column)
        {
            if (column.Width > 0)
            {
                if (column.MinWidth > 0) return Math.Min(column.Width, column.MinWidth);

                return column.Width;
            }

            return column.MinWidth;
        }

        public static string DecimalSeparator()
        {
            return CurrentCulture().NumberFormat.CurrencyDecimalSeparator;
        }

        public static CultureInfo CurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture;
        }

        public static string RemExtraDecimals(string val, string sep, int decimals)
        {
            var pos = val.IndexOf(sep, StringComparison.Ordinal);
            if (pos == -1) return val;

            var flen = val.Length - pos - 1;

            if (decimals < flen)
            {
                var dif = flen - decimals;
                val = val.Substring(0, val.Length - dif);
            }

            return val;
        }

        public static void AddArray<T>(this IList<T> target, IEnumerable<T> source)
        {
            if (source != null)
            {
                foreach (var v in source)
                {
                    target.Add(v);
                }
            }
        }

        /// <summary>
        /// Gets the value of one or more properties which names are specified separated by comma; e.g. "FirstName,LastName"
        /// </summary>
        /// <param name="name">comma separated property names</param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<object> GetColumnValue(string name, object o)
        {
            var columns = name.Split(',');
            return columns.Select(c => GetColumnValue(c.Split('.').ToList(), o));
        }

        private static object GetColumnValue(IList<string> trail, object o)
        {
            var prop = TypeDescriptor.GetProperties(o.GetType()).Find(trail[0], false);
            if (prop == null) throw new InvalidDataException("there is no Property named:" + trail[0] + " in type " + o.GetType().Name);
            var val = prop.GetValue(o);

            if (trail.Count == 1)
            {
                return val;
            }

            if (val == null) return null;
            trail.RemoveAt(0);
            return GetColumnValue(trail, val);
        }

        public static string GetColumnItmStr<T>(Column<T> c, T itm)
        {
            if (itm is null) return null;

            if (c.GetStr != null)
            {
                return c.GetStr(itm);
            }
            else if (c.Bind is not null)
            {
                return ColumnBindValsToStr(GetColumnValue(c.Bind, itm));
            }

            return string.Empty;
        }

        public static string ColumnBindValsToStr(IEnumerable<object> vals, string separator = " ")
        {
            var result = string.Empty;
            if (vals == null) return result;

            var notfirst = false;
            foreach (var val in vals.Where(l => l != null))
            {
                if (notfirst) result += separator;
                notfirst = true;
                var type = val.GetType();
                if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    var dtv = (DateTime?)val;
                    result += dtv.Value.ToShortDateString();
                }
                else
                {
                    result += val;
                }
            }

            return result;
        }

        public static string HtmlEncode(string input)
        {
            return HttpUtility.HtmlEncode(input);
        }
    }
}