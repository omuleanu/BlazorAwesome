using Microsoft.AspNetCore.Components;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid reorder rows options
    /// </summary>
    public class GridReorderRowsOpt
    {
        /// <summary>
        /// Grid rows container
        /// </summary>
        public ElementReference? Cont { get; set; }

        /// <summary>
        /// Object reference for the object that contains the JSInvokable drop method
        /// </summary>
        public object ObjRef { get; set; }

        /// <summary>
        /// Name of the JSInvokable drop method
        /// </summary>
        public string DropMethod { get; set; }

        /// <summary>
        /// Selector for the drag handle
        /// </summary>
        public string HandleSelector { get; set; }
    }
}