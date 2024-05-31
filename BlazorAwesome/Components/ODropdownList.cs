using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Omu.BlazorAwesome.Models.Utils;
using Omu.BlazorAwesome.Components.Internal;

namespace Omu.BlazorAwesome.Components
{
    /// <summary>
    /// Multiselect component
    /// </summary>
    /// <typeparam name="TKey">Data Item Key property, and Value Type</typeparam>
    public class OMultiselect<TKey> : OMulticheck<TKey>
    {
        /// <summary>        
        /// </summary>
        public OMultiselect()
        {
            multiselect = true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override string typeClass => "o-multiselect";
    }

    /// <summary>
    /// Multicheck component
    /// </summary>
    /// <typeparam name="TKey">Data Item Key property, and Value collection generic argument type</typeparam>
    public class OMulticheck<TKey> : ODropdown<TKey>
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override string typeClass => "o-multichk";

        /// <summary>        
        /// </summary>
        public OMulticheck()
        {
            multiple = true;
        }

        /// <summary>        
        /// </summary>
        [Parameter]
        public IEnumerable<TKey> Value
        {
            set
            {
                values.Clear();
                if (value != null)
                {
                    values.AddArray(value.Cast<object>());
                }
            }

            get
            {
                return values.Cast<TKey>();
            }
        }

        /// <summary>        
        /// </summary>
        [Parameter]
        public EventCallback<IEnumerable<TKey>> ValueChanged { get; set; }

        /// <summary> 
        /// <inheritdoc/>
        /// </summary>
        protected override async Task ValueChangedInvoke()
        {
            var v = values.ToArray();
            await ValueChanged.InvokeAsync(v.Any() ? v.Cast<TKey>().ToArray() : null);
            await triggerChange();
        }
    }

    /// <summary>
    /// DropdownList component
    /// </summary>
    /// <typeparam name="TKey">Key Type</typeparam>
    public class ODropdownList<TKey> : ODropdown<TKey>
    {
        /// <summary>        
        /// <inheritdoc/>
        /// </summary>
        protected override string typeClass => "o-ddlist";

        /// <summary>        
        /// </summary>
        [Parameter]
        public TKey Value
        {
            set
            {
                values.Clear();
                if (value != null)
                {
                    values.AddArray(new object[] { value });
                }
            }

            get
            {
                return (TKey)(values.Count == 0 ? null : values[0]);
            }
        }

        /// <summary>        
        /// </summary>
        [Parameter]
        public EventCallback<TKey> ValueChanged { get; set; }

        /// <summary>        
        /// <inheritdoc/>
        /// </summary>
        protected override async Task ValueChangedInvoke()
        {
            var val = values.FirstOrDefault();

            // object[] values -> TKey cval
            TKey cval;
            if (val is TKey cval1)
            {
                cval = cval1;
            }
            else
            {
                cval = default;
            }

            await ValueChanged.InvokeAsync(cval);
            await triggerChange();
        }
    }
}
