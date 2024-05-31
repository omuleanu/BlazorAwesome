using System;
using System.Threading.Tasks;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// TextboxOpt config model
    /// </summary>
    public class TextboxOpt : EditorWithClearOpt
    {
        /// <summary>
        /// Value format function
        /// </summary>
        public Func<string, string> FormatFunc { get; set; }

        /// <summary>
        /// Input placeholder
        /// </summary>
        public string Placeholder { get; set; }
    }    
}