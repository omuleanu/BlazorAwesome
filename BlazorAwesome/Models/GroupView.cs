using System.Collections.Generic;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid group view (node)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupView<T>
    {
        /// <summary>
        /// group header
        /// </summary>
        public GroupHeader<T> Header { get; set; }

        /// <summary>
        /// Items
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Groups
        /// </summary>
        public IEnumerable<GroupView<T>> Groups { get; set; }

        /// <summary>
        /// Current depth
        /// </summary>
        public int Depth { get; set; }

        /// <summary>        
        /// </summary>
        public NodeType NodeType { get; set; }

        /// <summary>        
        /// </summary>
        public GroupFooter Footer { get; set; }

        /// <summary>
        /// Group key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Column.Id values grouped by
        /// </summary>
        public IEnumerable<string> GroupColumns { get; set; }
    }
}