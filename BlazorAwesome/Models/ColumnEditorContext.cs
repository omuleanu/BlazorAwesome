namespace Omu.BlazorAwesome.Models
{
    /// <summary>    
    /// </summary>    
    public class ColumnEditorContext<T>
    {
        /// <summary>
        /// Edit item state
        /// </summary>
        public EditItemState<T> EditItemState { get; set; }

        /// <summary>
        /// Grid column
        /// </summary>
        public Column<T> Column { get; set; }
    }
}