using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SListOpt
    {
        /// <summary>
        /// Custom item render function
        /// </summary>
        public Func<KeyContent, RenderFragment> ItemRender { get; set; }

        /// <summary>
        /// Use single select instead of multiple
        /// </summary>
        public bool SingleSelect { get; set; }

        /// <summary>
        /// List items data
        /// </summary>
        public IEnumerable<KeyContent> Data { get; set; }

        /// <summary>
        /// Opener ref
        /// </summary>
        public ElementReference Opener { get; set; }

        /// <summary>
        /// Item selected func
        /// </summary>
        public Func<ItemSelectedPrm, Task> OnItemSelected { get; set; }

        /// <summary>
        /// Css style for the items container
        /// </summary>
        public string ItemsContStyle { get; set; }

        /// <summary>        
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Filter predicate, default is item.Content.Contains(str, StringComparison.InvariantCultureIgnoreCase)
        /// </summary>
        public Func<KeyContent, string, bool> FilterPredicate { get; set; }

        /// <summary>
        /// Focused item css class
        /// </summary>
        public string FocusClass { get; set; }

        /// <summary>
        /// Item css class
        /// </summary>
        public string ItemClass { get; set; }

        /// <summary>
        /// Selected item css class
        /// </summary>
        public string SelectedClass { get; set; }

        /// <summary>
        /// Focus item on hover
        /// </summary>
        public bool ItemHoverFocus { get; set; }

        /// <summary>
        /// Show combo item on search
        /// </summary>
        public bool ComboItem { get; set; }
    }
}