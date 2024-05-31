using System;
using Microsoft.AspNetCore.Components;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid editor
    /// </summary>    
    public class GridEditor<TModel>
    {
        /// <summary>
        /// Grid column render function
        /// </summary>
        public Func<ColumnEditorContext<TModel>, RenderFragment> Render { get; set; }
    }
}