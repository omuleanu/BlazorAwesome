using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid filter
    /// </summary>    
    public class GridFilter<TModel>
    {   
        /// <summary>
        /// Filter render func
        /// </summary>
        public Func<ColumnState<TModel>, RenderFragment> Render { get; set; }

        /// <summary>
        /// Css class for the filter container
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Filter query (object value, Expression Where predicate)
        /// </summary>
        public Func<object, Expression<Func<TModel, bool>>> Query { get; set; }

        /// <summary>
        /// Get data for the filter 
        /// </summary>
        public Func<FilterContext<TModel>, Task<object>> GetData { get; set; }        

        /// <summary>
        /// Set filter data after query was executed
        /// </summary>
        public bool SelfFilter { get; set; }
    }
}