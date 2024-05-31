using System.Collections.Generic;

namespace Omu.BlazorAwesome.Models.Utils
{
    /// <summary>
    /// Inline Dropdown Parameters
    /// </summary>
    public class InlDropdownPrm
    {
        /// <summary>
        /// Model property name, default is value of Column.Bind
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Dropdown options
        /// </summary>
        public DropdownOpt Opt { get; set; }
    }
}