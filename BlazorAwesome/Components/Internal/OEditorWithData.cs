using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Omu.BlazorAwesome.Models;

namespace Omu.BlazorAwesome.Components.Internal
{
    /// <summary>
    /// Editor component with Data Parameter
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class OEditorWithData<TValue> : OEditorBase<TValue>
    {
        /// <summary>        
        /// </summary>
        public EditorState State { get; private set; }

        /// <summary>
        /// Check if value present in data, and remove if not
        /// </summary>        
        protected virtual Task checkValInData()
        {
            return Task.CompletedTask;
        }

        private void setupState()
        {
            if (State is not null)
            {
                return;
            }

            State = new();
            State.GetOpt = () => this.Opt;
            State.CheckVal = checkValInData;

            if (prmData is not null)
            {
                State.Data = prmData;
                prmData = null;
            }

            if (Opt?.ParentValueFunc is not null)
            {
                parentValue = AweUtil.Serialize(Opt.ParentValueFunc());
            }
        }

        /// <summary>
        /// Editor options
        /// </summary>
        [Parameter]
        public DataEditorOpt Opt { get; set; }

        private IEnumerable<KeyContent> prmData;

        /// <summary>
        /// setting parameter data
        /// </summary>
        protected void setData(IEnumerable<KeyContent> data)
        {
            if (State != null)
            {
                State.Data = data;
            }

            prmData = data;
        }

        /// <summary>
        /// Data
        /// </summary>
        [Parameter]
        public IEnumerable<KeyContent> Data
        {
            set
            {
                setData(value);
            }

            get
            {
                if (State != null)
                {
                    return State.Data;
                }

                return prmData;
            }
        }

        /// <summary>
        /// setting parameter data
        /// </summary>
        protected IEnumerable<KeyContent> getData => State?.Data ?? Opt?.Data ?? prmData;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>        
        protected override IEditorOpt getOpt()
        {
            return Opt;
        }

        /// <summary>
        /// value used to detect if parent components changed value
        /// </summary>
        protected string parentValue;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>        
        protected override async Task OnInitializedAsync()
        {
            setupState();

            await State?.LoadAsync();
            await checkValInData();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override async Task OnParametersSetAsync()
        {
            //await checkValInData();

            await parentReload();
        }

        /// <summary>
        /// Call state load if parentValue changed
        /// </summary>        
        protected async Task parentReload()
        {
            if (Opt?.ParentValueFunc is not null)
            {
                var crval = AweUtil.Serialize(Opt.ParentValueFunc());
                if (crval != parentValue)
                {
                    parentValue = crval;

                    await State.LoadAsync();
                }
            }
        }
    }
}