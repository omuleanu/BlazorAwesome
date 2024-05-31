using System;
using System.Threading.Tasks;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// PagerOpt config model
    /// </summary>
    public class PagerOpt
    {
        /// <summary>
        /// Initial page
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Page count
        /// </summary>
        public int PageCount { get; set; }
        
        /// <summary>
        /// Action on page click
        /// </summary>
        public Func<int, Task> Action { get; set; }
    }
}