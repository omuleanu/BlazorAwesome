using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Inline editing grid functionality
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InlineEditOpt<T>
    {
        /// <summary>
        /// Get edit state input model
        /// </summary>
        public Func<T, object> GetModel { get; set; }

        /// <summary>
        /// Save item function
        /// </summary>
        public Func<EditItemState<T>, Task<bool>> Save { get; set; }

        /// <summary>
        /// Save all items function
        /// </summary>
        public Func<IEnumerable<EditItemState<T>>, Task<IEnumerable<string>>> SaveAll { get; set; }

        /// <summary>
        /// Set a function that will execute after an EditItemState object is initialized, 
        /// (used to call EnableDataAnnotationsValidation on the .EditContext)
        /// </summary>
        public Action<EditItemState<T>> InitItem { get; set; }        
    }
}