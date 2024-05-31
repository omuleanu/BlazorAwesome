using System.Collections.Generic;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid Group Info
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupInfo<T>
    {
        /// <summary>
        /// Group Items or Node children
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Column grouped by
        /// </summary>
        public Column<T> Column { get; set; }

        /// <summary>
        /// Lazy node
        /// </summary>
        public bool Lazy { get; internal set; }
        
        /// <summary>
        /// 
        /// </summary>
        public T NodeItem { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public int Level { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public int NodeLevel { get; internal set; }
    }
}