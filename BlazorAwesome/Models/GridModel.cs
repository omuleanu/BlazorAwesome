namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid model
    /// </summary>    
    public class GridModel<T>
    {
        /// <summary>
        /// Data
        /// </summary>
        public GroupView<T> Data { get; set; }

        /// <summary>
        /// Number of pages
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TreeHeight { get; set; }

        /// <summary>
        /// Total items count, for all pages
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// Number of groups
        /// </summary>
        public int GroupsCount { get; set; }
    }
}