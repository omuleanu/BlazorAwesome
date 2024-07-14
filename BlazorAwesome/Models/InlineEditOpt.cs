using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Inline editing grid optioms
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class InlineEditOpt<T>
    {
        /// <summary>
        /// Function that will return the model for the inline edit state input object, 
        /// the object used to hold the editable values of the inline edit item state (like textbox string value, dropdown selected key, ... )
        /// </summary>
        public Func<T, object> GetModel { get; set; }

        /// <summary>
        /// Save item function, gets the EditItemState as parameter and should return true on success
        /// </summary>
        public Func<EditItemState<T>, Task<bool>> Save { get; set; }

        /// <summary>
        /// Save all items function, gets the collection of EditItemStates and should return a collection of keys of the successfully saved items;
        /// so items that failed to be saved due to validation or other errors shouldn't have their key added to that collection
        /// </summary>
        public Func<IEnumerable<EditItemState<T>>, Task<IEnumerable<string>>> SaveAll { get; set; }

        /// <summary>
        /// Set a function that will execute after an EditItemState object is initialized, 
        /// (used to call EnableDataAnnotationsValidation on the .EditContext)
        /// </summary>
        public Action<EditItemState<T>> InitItem { get; set; }

        /// <summary>
        /// Edit row on click 
        /// </summary>
        public bool RowClickEdit { get; set; }
    }
}