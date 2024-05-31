using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Dropdown editors options
    /// </summary>
    public class DropdownOpt : EditorWithClearOpt, IParentValueOpt, IDataOpt
    {
        /// <summary>
        /// Main caption render func
        /// </summary>
        public Func<RenderFragment> MainCaptionFunc { get; set; }

        /// <summary>
        /// Item caption custom render func
        /// </summary>
        public Func<KeyContent, RenderFragment> CaptionFunc { get; set; }

        /// <summary>
        /// Select item render func
        /// </summary>
        public Func<KeyContent, RenderFragment> ItemFunc { get; set; }

        /// <summary>
        /// Label rendered next to the editor's selected value (caption)
        /// </summary>
        public string InLabel { get; set; }

        /// <inheritdoc/>
        public Func<Task<IEnumerable<KeyContent>>> DataFunc { get; set; }

        /// <inheritdoc/>
        public IEnumerable<KeyContent> Data { get; set; }

        /// <summary>
        /// Open dropdown on editor hover
        /// </summary>
        public bool OpenOnHover { get; set; }

        /// <summary>
        /// Function to execute after the user stopped typing in the search textbox, 
        /// gets the search term as parameter
        /// </summary>
        public Func<string, Task<bool>> SearchFunc { get; set; }

        /// <summary>
        /// No close on item select
        /// </summary>
        public bool? NoSelectClose { get; set; }

        /// <inheritdoc/>
        public Func<object> ParentValueFunc { get; set; }

        /// <summary>
        /// CSS witdh
        /// </summary>
        public string Width { get; set; }
    }

    /// <summary>
    /// ParentValue containing opt model
    /// </summary>
    public interface IParentValueOpt
    {
        /// <summary>
        /// Function that returns the current value of parent components; when the parent value changes the component will reload, 
        /// (for multiple parents return an array/collection)
        /// </summary>
        public Func<object> ParentValueFunc { get; set; }
    }

    /// <summary>
    /// Data containing opt model
    /// </summary>
    public interface IDataOpt
    {
        /// <summary>
        /// Function that will execute on component initialization, it will return the data for the component
        /// </summary>
        public Func<Task<IEnumerable<KeyContent>>> DataFunc { get; set; }

        /// <summary>
        /// Component data used for selecting values
        /// </summary>
        public IEnumerable<KeyContent> Data { get; set; }
    }

    /// <summary>
    /// Editor with clear button
    /// </summary>
    public class EditorWithClearOpt : EditorOpt
    {
        /// <summary>
        /// Clear button
        /// </summary>
        public bool? ClearBtn { get; set; }
    }

    /// <summary>
    /// Select list options
    /// </summary>
    public class SelectListOpt : EditorOpt
    {
        /// <summary>
        /// Can select multiple values
        /// </summary>
        public bool Multiple { get; set; }

        /// <summary>
        /// Item render function
        /// </summary>
        public Func<KeyContent, RenderFragment> ItemFunc { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public IEnumerable<KeyContent> Data { get; set; }

        /// <summary>
        /// Items container css style
        /// </summary>
        public string ItemsContStyle { get; set; }

        /// <summary>
        /// Filter predicate, default is item.Content.Contains(filter string)
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
    }

    /// <summary>
    /// Toggle options
    /// </summary>
    public class ToggleOpt : EditorOpt
    {
        /// <summary>
        /// Yes text
        /// </summary>
        public string Yes { get; set; }

        /// <summary>
        /// No text
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// Width css value
        /// </summary>
        public string Width { get; set; }
    }

    /// <summary>
    /// Editor options
    /// </summary>
    public class EditorOpt : IEditorOpt
    {
        /// <summary>
        /// Is editor disabled
        /// </summary>
        public bool? Disabled { get; set; }

        /// <summary>
        /// Main container css class
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Function to execute after value has changed
        /// </summary>
        public Func<Task> AfterChangeFunc { get; set; }
    }

    /// <summary>
    /// Context options
    /// </summary>
    public class ContextOpt
    {
        /// <summary>
        /// Disabled
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Clear button
        /// </summary>
        public bool ClearBtn { get; set; }

        /// <summary>
        /// Function to execute after value has changed
        /// </summary>
        public Func<Task> AfterChangeFunc { get; set; }
    }

    /// <summary>
    /// Editor with data options
    /// </summary>
    public class DataEditorOpt : EditorOpt, IParentValueOpt, IDataOpt
    {
        /// <inheritdoc/>
        public Func<Task<IEnumerable<KeyContent>>> DataFunc { get; set; }

        /// <inheritdoc/>
        public IEnumerable<KeyContent> Data { get; set; }

        /// <inheritdoc/>
        public Func<object> ParentValueFunc { get; set; }
    }
}