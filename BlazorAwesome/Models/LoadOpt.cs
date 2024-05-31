namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Load options
    /// </summary>
    public class LoadOpt
    {
        /// <summary>
        /// Partial loading
        /// </summary>
        public bool Partial { get; set; }

        /// <summary>
        /// Lazy item
        /// </summary>
        public object LazyItem { get; set; }
    }
}