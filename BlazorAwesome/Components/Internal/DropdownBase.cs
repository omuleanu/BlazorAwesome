using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Omu.BlazorAwesome.Models;
using Omu.BlazorAwesome.Models.Utils;

namespace Omu.BlazorAwesome.Components.Internal
{
    /// <summary>
    /// DropdownBase
    /// </summary>
    public abstract class DropdownBase : OEditorBase<object>
    {
        /// <summary>
        /// 
        /// </summary>
        public string log = string.Empty;

        /// <summary>
        /// values
        /// </summary>
        protected List<object> values = new();

        /// <summary>
        /// get value
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetValue() => values;

        /// <summary>
        /// is combo
        /// </summary>
        protected bool combo;

        //public async Task SetValue(IEnumerable<object> nvals)
        //{
        //    values.Clear();
        //    values.AddArray(nvals);
        //    await ValueChangedInvoke();
        //}

        /// <summary>
        /// Call StateHasChanged on the component
        /// </summary>
        public void Render()
        {
            StateHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Task ValueChangedInvoke()
        {
            return Task.CompletedTask;
        }
    }
}