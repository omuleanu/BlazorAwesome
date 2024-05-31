using Omu.BlazorAwesome.Models.Utils;
using System.Collections.Generic;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid state dto used for saving/loading state
    /// </summary>
    public class GridStateDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, AweJsonObj> FilterValues { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ColumnStateDto[] ColumnsStates { get; set; }
    }
}