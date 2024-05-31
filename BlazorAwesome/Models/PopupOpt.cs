using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// PopupOpt config model
    /// </summary>
    public class PopupOpt
    {
        /// <summary>
        /// Open popup on init
        /// </summary>
        public bool AutoOpen { get; set; }

        private bool? outClickClose;

        /// <summary>
        /// Popup title, rendered in the popup header
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Is the popup a dropdown
        /// </summary>
        public bool Dropdown { get; set; }

        /// <summary>
        /// Use Menu popup inner html (like the DropdownList for example)
        /// </summary>
        public bool Menu { get; set; }

        /// <summary>
        /// Css class for the popup div
        /// </summary>
        public string PopupClass { get; set; }

        /// <summary>
        /// Is the popup a modal
        /// </summary>
        public bool Modal { get; set; }

        /// <summary>
        /// Close the popup when clicking outside of it (defaults to true when Dropdown is true)
        /// </summary>
        public bool OutClickClose
        {
            get => outClickClose ?? Dropdown;
            set => outClickClose = value;
        }

        /// <summary>        
        /// </summary>
        public int Width { get; set; }

        /// <summary>        
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Function to execute on popup close
        /// </summary>
        public Func<Task> OnClose { get; set; }

        /// <summary>
        /// Function to execute on popup open
        /// </summary>
        public Func<Task> OnOpen { get; set; }

        /// <summary>
        /// Don't focus opener on close
        /// </summary>
        public bool NoCloseFocus { get; set; }

        /// <summary>
        /// Don't focus first tabbale on open
        /// </summary>
        public bool NoFocus { get; set; }
    }

    /// <summary>
    /// PopupFormOpt config model
    /// </summary>
    public class PopupFormOpt : PopupOpt
    {
        /// <summary>
        /// Text for the ok button
        /// </summary>
        public string OkText { get; set; }

        /// <summary>
        /// Text for the cancel button
        /// </summary>
        public string CancelText { get; set; }
    }
}