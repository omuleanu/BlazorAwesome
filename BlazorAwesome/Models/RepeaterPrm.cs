namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Repeater iteration parameter
    /// </summary>
    /// <typeparam name="TItem">Loop item type</typeparam>
    public class RepeaterPrm<TItem>
    {
        /// <summary>        
        /// </summary>
        public TItem Item { get; set; }

        /// <summary>        
        /// </summary>
        public bool IsLast { get; set; }
    }
}