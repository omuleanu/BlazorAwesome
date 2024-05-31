using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>    
    /// Grid inline edit item state
    /// </summary>    
    public class EditItemState<T>
    {
        /// <summary>
        /// Edit item is a create action
        /// </summary>
        public bool IsCreate => Item is null;

        /// <summary>
        /// Item key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// State Item 
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        /// State input
        /// </summary>
        public object Input { get; set; }

        /// <summary>        
        /// </summary>
        public EditContext EditContext { get; set; }

        /// <summary>
        /// Parent key (when there is a parent node)
        /// </summary>
        public string ParentKey { get; set; }        
    }
}