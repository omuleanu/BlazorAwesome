using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Drag and drop options
    /// </summary>
    public class DragOpt
    {
        /// <summary>
        /// Do not use placeholder
        /// </summary>
        public bool NoPlaceholder { get; set; }

        /// <summary>
        /// Do not hide the drag source
        /// </summary>
        public bool NoSourceHide { get; set; }

        /// <summary>
        /// Use sticky placeholder
        /// </summary>
        public bool StickyPlaceholder { get; set; }

        /// <summary>
        /// Drag from container
        /// </summary>
        public ElementReference? Cont { get; set; }

        /// <summary>
        /// Drop container
        /// </summary>
        public ElementReference? ToCont { get; set; }

        /// <summary>
        /// Drag from selector
        /// </summary>
        public string FromSelector { get; set; }

        /// <summary>
        /// Drag object selector
        /// </summary>
        public string Selector { get; set; }

        /// <summary>
        /// Drag handle selector
        /// </summary>
        public string HandleSelector { get; set; }

        /// <summary>
        /// Drag to selector
        /// </summary>
        public string ToSelector { get; set; }

        /// <summary>
        /// Object reference for the object that contains the JSInvokable drop method
        /// </summary>
        public object ObjRef { get; set; }

        /// <summary>
        /// Name of the JSInvokable drop method
        /// </summary>
        public string DropMethod { get; set; }

        /// <summary>
        /// Don't reorder drop items
        /// </summary>
        public bool NoReorder { get; set; }

        /// <summary>
        /// Change dom on drop, 
        /// when using ChangeDom you must change @key of drag container before each drop StateChange
        /// </summary>
        public bool ChangeDom { get; set; }

        /// <summary>
        /// Css class of the placeholder
        /// </summary>
        public string PlaceholderClass { get; set; }

        /// <summary>
        /// Name of a JS func used to create the drag object
        /// </summary>
        public string DragWrapJSFunc { get; set; }

        /// <summary>
        /// js reference to the previous drag and drop initialization, 
        /// set to remove drag functionality before setting it again
        /// </summary>
        public IJSObjectReference DragRef { get; set; }

        /// <summary>
        /// </summary>        
        public object ToDto()
        {
            return new
            {
                objRef = ObjRef,
                method = DropMethod,
                fromCont = Cont is not null ? Cont : null,
                dropConts = ToCont is not null ? new[] { ToCont } : null,
                from = FromSelector,
                to = ToSelector,
                sel = Selector,
                noHide = NoSourceHide,
                noPlh = NoPlaceholder,
                noReorder = NoReorder,
                splh = StickyPlaceholder,
                plhCls = PlaceholderClass,
                handle = HandleSelector,
                swrap = DragWrapJSFunc,
                chDom = ChangeDom,
                dragRef = DragRef
            };
        }
    }
}