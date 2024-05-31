namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid group header model
    /// </summary>
    public class GroupHeader<T>
    {
        /// <summary>
        /// Content of the Group Header
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// If true the group will be collapsed
        /// </summary>
        public bool Collapsed { get; set; }

        /// <summary>
        /// Grid header item (set when using tree grid)
        /// </summary>
        public T Item { get; set; }
    }
}