namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid GroupView Data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupViewData<T>
    {
        /// <summary>        
        /// </summary>
        public GridOpt<T> Opt { get; set; }

        /// <summary>        
        /// </summary>
        public GridState<T> State { get; set; }
    }
}