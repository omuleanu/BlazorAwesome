using Microsoft.AspNetCore.Components.Forms;
using Omu.BlazorAwesome.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>    
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// get gridstate filter data by key
        /// </summary>
        public static object GetFilterData<T>(this GridState<T> gridState, string key)
        {
            if (gridState.FilterData == null) return null;
            if (!gridState.FilterData.ContainsKey(key)) return null;

            return gridState.FilterData[key];
        }

        /// <summary>
        /// check if the grid has key set
        /// </summary>        
        public static bool HasKey<T>(this GridOpt<T> gopt)
        {
            return gopt.Key is not null || gopt.GetKeyFunc is not null;
        }

        /// <summary>
        /// add grid column
        /// </summary>        
        public static Column<T> Column<T>(this GridOpt<T> gopt, Column<T> column)
        {
            if (gopt.Columns is null)
            {
                gopt.Columns = new List<Column<T>>();
            }

            gopt.Columns.Add(column);

            return column;
        }

        /// <summary>
        /// Set grid state count and reset page to 1 if out of bounds
        /// </summary>
        public static void SetCount<T>(this GridState<T> state, long count)
        {
            state.Count = count;
            // go to page 1 when no results on current page
            if (state.Page > 1 && count <= (state.Page - 1) * state.PageSize)
            {
                state.Page = 1;
            }
        }

        /// <summary>
        /// Order by, and get page
        /// </summary>
        public static IQueryable<T> QueryPage<T>(this GridState<T> state, IQueryable<T> q)
        {
            return AweUtil.GetPage(state.OrderBy(q), state.Page, state.PageSize);
        }

        /// <summary>
        /// Apply grid filters to the query
        /// </summary>        
        public static async Task<IQueryable<T>> ApplyFilters<T>(
            this GridState<T> gridState,
            IQueryable<T> query,
            object data = null)
        {
            async Task setFilterData(Column<T> column, IQueryable<T> q)
            {
                gridState.FilterData[column.Id] = await column.GetFilter().GetData(new() { Query = q, Data = data });
            };

            // apply rules in filter order (order picked by the user)
            foreach (var prop in gridState.FilterValues)
            {
                if (prop.Value is null) continue;

                var columnState = gridState.ColumnsStates.FirstOrDefault(o => o.Id == prop.Key);

                if (columnState == null) continue;

                var filter = columnState.Column.GetFilter();

                if (filter.GetData != null && !filter.SelfFilter)
                {
                    await setFilterData(columnState.Column, query);
                }

                if (filter.Query == null)
                {
                    throw new AwesomeException($"column {columnState.Id} {nameof(filter.Query)} not specified");
                }
                else
                {
                    query = query.Where(filter.Query(prop.Value)).AsQueryable();
                }

                if (filter.GetData != null && filter.SelfFilter)
                {
                    await setFilterData(columnState.Column, query);
                }
            }

            // populate the rest (that don't have a filter value set) of the filterRow editors
            var rest = gridState.ColumnsStates
                .Where(cs => cs.Column.GetFilter()?.GetData != null && !gridState.FilterValues.ContainsKey(cs.Id))
                .ToArray();

            foreach (var column in rest)
            {
                await setFilterData(column.Column, query);
            }

            return query;
        }

        /// <summary>
        /// Get grid filter value
        /// </summary>        
        public static object GetFilterValue<T>(this GridState<T> gs, string name)
        {
            return gs.FilterValues.ContainsKey(name) ? gs.FilterValues[name] : null;
        }

        /// <summary>
        /// Change grid filter value and load grid
        /// </summary>        
        public static async Task FilterValChange<T>(this GridState<T> gs, string name, object val)
        {
            if (val != null)
            {
                gs.FilterValues[name] = val;
            }
            else if (gs.FilterValues.ContainsKey(name))
            {
                gs.FilterValues.Remove(name);
            }

            gs.GridStateChanged();
            await gs.LoadAsync();
        }

        /// <summary>
        /// Clear filters value and load grid
        /// </summary>        
        public static async Task ClearFilterValsForAndLoad<T>(this GridState<T> gs, string name)
        {
            ClearFilterValsFor(gs, name + "_Op");

            if (!ClearFilterValsFor(gs, name)) return;

            gs.GridStateChanged();
            await gs.LoadAsync();
        }

        /// <summary>
        /// Clear filter value
        /// </summary>        
        public static bool ClearFilterValsFor<T>(this GridState<T> gs, string name)
        {
            var removed = false;
            if (gs.FilterValues.ContainsKey(name))
            {
                gs.FilterValues.Remove(name);
                removed = true;
            }

            foreach (var key in gs.FilterValues.Keys.Where(o => o.StartsWith(name + "-")))
            {
                gs.FilterValues.Remove(key);
                removed = true;
            }

            return removed;
        }
    }
}