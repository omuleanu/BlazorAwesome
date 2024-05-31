using Microsoft.AspNetCore.Components;
using System;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid nest
    /// </summary>
    public class Nest
    {
        /// <summary>
        /// Nest key
        /// </summary>
        public object Key { get; set; }

        /// <summary>
        /// Nest name, calling ToggleOpen for a nest with a the same key and different name will close the existing and open the new one
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nest content
        /// </summary>
        public RenderFragment Render { get; set; }

        /// <summary>
        /// Open nest on top of the grid content
        /// </summary>
        public bool IsTop { get; set; }
    }
}