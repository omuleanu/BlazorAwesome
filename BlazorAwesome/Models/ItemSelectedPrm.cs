using System;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Item selected parameters
    /// </summary>
    public class ItemSelectedPrm
    {
        /// <summary>
        /// Selection as a result of user click
        /// </summary>
        public bool UserClick { get; set; }

        /// <summary>
        /// Selected values
        /// </summary>
        public object[] Value { get; set; }
    }
}