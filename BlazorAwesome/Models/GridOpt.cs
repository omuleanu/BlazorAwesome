using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Omu.BlazorAwesome.Components;
using Omu.BlazorAwesome.Core;
using Omu.BlazorAwesome.Models.Utils;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid options model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridOpt<T> : IGridStateProp<T>
    {
        private IList<Column<T>> columns;

        /// <summary>
        /// Function for creating a group footer
        /// </summary>
        public Func<GroupInfo<T>, GroupFooter> MakeFooter { get; set; }

        /// <summary>
        /// Function for creating a group header
        /// </summary>
        public Func<GroupInfo<T>, GroupHeader<T>> MakeHeader { get; set; }

        /// <summary>
        /// Load grid on init
        /// </summary>
        public bool Load { get; set; } = true;

        /// <summary>
        /// CSS class for the main div
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Default value for Column.Groupable
        /// </summary>
        public bool Groupable { get; set; } = true;

        /// <summary>
        /// Default value for Column.Sortable
        /// </summary>
        public bool Sortable { get; set; } = true;

        /// <summary>
        /// Default value for Column.Reorderable
        /// </summary>
        public bool Reorderable { get; set; } = true;

        /// <summary>
        /// Default value for Column.Resizable
        /// </summary>
        public bool? Resizable { get; set; } = true;

        /// <summary>
        /// Grid columns
        /// </summary>
        public IList<Column<T>> Columns { get => columns; set => columns = value.ToList(); }

        /// <summary>
        /// Enable filter row
        /// </summary>
        public bool FilterRow { get; set; }

        /// <summary>
        /// Grid height
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Grid content height
        /// </summary>
        public int? ContentHeight { get; set; }

        /// <summary>
        /// Default column width
        /// </summary>
        public int ColumnWidth { get; set; } = 140;

        /// <summary>
        /// Function to set row css class based on row model
        /// </summary>
        public Func<T, string> RowClassFunc { get; set; }

        /// <summary>
        /// Row on click function, gets row key as parameter
        /// </summary>
        public Action<string> RowClickFunc { get; set; }

        /// <summary>
        /// Action to execute on begin load
        /// </summary>
        public Action BeginLoadFunc { get; set; }

        /// <summary>
        /// Action to execute on load
        /// </summary>
        public Action LoadFunc { get; set; }
        
        /// <summary>
        /// Frozen columns on the left
        /// </summary>
        public int FrozenColumnsLeft { get; set; }

        /// <summary>
        /// Frozen columns on the right
        /// </summary>
        public int FrozenColumnsRight { get; set; }

        /// <summary>
        /// Initial page
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Initial page size
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Get intial query func; query before we apply FilterRow filtering, sorting, paging
        /// </summary>
        public Func<IQueryable<T>> GetQuery { get; set; }

        /// <summary>
        /// Grid key property name
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Default sort type
        /// </summary>
        public Sort DefaultKeySort { get; set; } = Sort.Desc;

        /// <summary>
        /// set grid key
        /// </summary>
        public Expression<Func<T, object>> KeyProp
        {
            set
            {
                Key = AweExprUtil.GetExpressionStr(value);
            }
        }

        /// <summary>
        /// Get get key from row model func
        /// </summary>
        public Func<T, object> GetKeyFunc { get; set; }

        /// <summary>
        /// Load data function
        /// </summary>
        public Func<Task> LoadData { get; set; }

        /// <summary>
        /// Get row children nodes given row model, and current node level
        /// </summary>
        public Func<T, int, object> GetChildren { get; set; }

        /// <summary>
        /// Function to execute when a lazy load node is expanded
        /// </summary>
        public Func<T, Task> LoadLazyNodeAsync { get; set; }

        /// <summary>
        /// Inline Editing options
        /// </summary>
        public InlineEditOpt<T> InlineEdit { get; set; }

        /// <summary>
        /// Get grid state
        /// </summary>
        public GridState<T> State { get; internal set; }

        /// <summary>
        /// Function to execute on grid state change
        /// </summary>
        public Func<Task> StateChangeFunc { get; set; }

        /// <summary>
        /// Custom order by
        /// </summary>
        public Func<IQueryable<T>, IQueryable<T>> OrderBy { get; set; }

        /// <summary>
        /// Custom func for getting column value
        /// </summary>
        public Func<string, object, IEnumerable<object>> GetBindValue { get; set; }
    }

    /// <summary>
    /// class has GridState property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGridStateProp<T>
    {
        /// <summary>
        /// Get grid state
        /// </summary>
        public GridState<T> State { get; }
    }
}