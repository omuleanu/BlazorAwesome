namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Drag and drop result
    /// </summary>
    public class DropRes
    {
        /// <summary>
        /// Drop item key
        /// </summary>
        public string ItmKey { get; set; }

        /// <summary>
        /// Drop item index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Drop container key
        /// </summary>
        public string ContKey { get; set; }

        /// <summary>
        /// Drag from container key
        /// </summary>
        public string FromContKey { get; set; }
    }
}