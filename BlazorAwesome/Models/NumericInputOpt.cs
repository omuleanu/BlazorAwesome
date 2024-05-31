using System;
using System.Threading.Tasks;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Numeric input options model
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class NumericInputOpt<TValue> : EditorWithClearOpt
    {
        private TValue max;
        private TValue min;

        /// <summary>
        /// 
        /// </summary>
        public bool UseMax { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool UseMin { get; private set; }

        /// <summary>
        /// Number of decimals
        /// </summary>
        public ushort Decimals { get; set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public TValue Max { get => max; set { max = value; UseMax = true; } }

        /// <summary>
        /// Minimum value
        /// </summary>
        public TValue Min { get => min; set { min = value; UseMin = true; } }
        
        /// <summary>
        /// Show input spin up/down buttons
        /// </summary>
        public bool? ShowSpinners { get; set; }

        /// <summary>
        /// Increase/decrease button step
        /// </summary>
        public float Step { get; set; } = 1;

        /// <summary>
        /// Display value format function, 
        /// will receive the value as parameter and returns the display value as string
        /// </summary>
        public Func<TValue, string> FormatFunc { get; set; }

        /// <summary>
        /// Input placeholder
        /// </summary>
        public string Placeholder { get; set; }
    }

    /// <summary>    
    /// </summary>
    public interface IEditorOpt
    {
        /// <summary>
        /// Is editor disabled
        /// </summary>
        bool? Disabled { get; set; }

        /// <summary>
        /// Editor css class
        /// </summary>
        string CssClass { get; set; }

        /// <summary>
        /// Function to execute after value has changed
        /// </summary>
        Func<Task> AfterChangeFunc { get; set; }
    }

    /// <summary>
    /// DatePicker options model
    /// </summary>
    public class DatePickerOpt : EditorWithClearOpt
    {
        /// <summary>
        /// Input placeholder
        /// </summary>
        public string Placeholder { get; set; }
    }
}