using System;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Awesome popup button config
    /// </summary>
    public class PopupButton
    {
        /// <summary>
        /// Button content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Action on button click
        /// </summary>
        public Action Click { get; set; }

        /// <summary>
        /// Add primary button css class
        /// </summary>
        public bool Primary { get; set; }
    }
}