using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Omu.BlazorAwesome.Components.Internal;
using Omu.BlazorAwesome.Models.Utils;

namespace Omu.BlazorAwesome.Components
{
    /// <summary>
    /// Combobox component
    /// </summary>
    public class OCombobox : ODropdown<object>
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string typeClass => "o-combobox";

        /// <summary>
        /// 
        /// </summary>
        public OCombobox()
        {
            combo = true;
        }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public object Value
        {            
            set
            {
                values.Clear();
                if (value != null)
                {
                    values.AddArray(new[] { value });
                }
            }

            get
            {
                return values.Count == 0 ? null : values[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public EventCallback<object> ValueChanged { get; set; }
                
        /// <inheritdoc/>
        protected override async Task ValueChangedInvoke()
        {
            await ValueChanged.InvokeAsync(values.FirstOrDefault());
            await triggerChange();
        }
    }
}