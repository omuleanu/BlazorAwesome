namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Column state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColumnState<T> : ColumnStateDto
    {
        /// <summary>
        /// 
        /// </summary>
        public Column<T> Column { get; set; }
    }
}