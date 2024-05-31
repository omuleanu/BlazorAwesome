namespace Omu.BlazorAwesome.Core
{
    /// <summary>
    /// Nullable generic value
    /// </summary>    
    public class NVal<TValue>
    {
        /// <summary>
        /// create NVal
        /// </summary>        
        public NVal(TValue val)
        {
            Value = val;
        }

        /// <summary>
        /// Value
        /// </summary>
        public TValue Value { get; set; }
    }
}