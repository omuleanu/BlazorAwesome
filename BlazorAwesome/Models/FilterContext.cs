using System.Linq;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid Filter Data Loading Context
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class FilterContext<TModel>
    {
        /// <summary>
        /// Grid data loading query
        /// </summary>
        public IQueryable<TModel> Query { get; set; }

        /// <summary>
        /// Additional data
        /// </summary>
        public object Data { get; set; }
    }
}