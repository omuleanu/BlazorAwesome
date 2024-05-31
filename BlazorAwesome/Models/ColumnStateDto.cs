using Omu.BlazorAwesome.Core;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Column state dto type (used for saving column state)
    /// </summary>
    public class ColumnStateDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Sort Sort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Rank { get; set; }

        /// <summary>
        /// is grouped
        /// </summary>
        public bool Group { get; set; }
    }
}