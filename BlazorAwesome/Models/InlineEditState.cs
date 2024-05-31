using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Omu.BlazorAwesome.Core;
using Omu.BlazorAwesome.Models.Utils;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid inline edit state
    /// </summary>    
    public class InlineEditState<T>
    {
        /// <summary>
        /// Set grid opt
        /// </summary>
        public GridOpt<T> GetOpt { private get; set; }

        private int createId = 0;
                
        /// <summary>
        /// ServiceProvider needed as parameter for EnableDataAnnotationsValidation
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Create inline edit item
        /// </summary>
        public void Create(object model, object parentKey = default)
        {
            var key = Consts.GridCreateNewPrefix + createId++;

            var itemState = new EditItemState<T>()
            {
                Key = key,
                Input = model,
                EditContext = new EditContext(model),
                ParentKey = parentKey?.ToString()
            };

            initItem(itemState);
            
            editStates.Add(itemState.Key, itemState);
        }

        private void initItem(EditItemState<T> itemState)
        {
            if (GetOpt.InlineEdit.InitItem != null)
            {
                GetOpt.InlineEdit.InitItem(itemState);
            }
            else
            {
                itemState.EditContext.EnableDataAnnotationsValidation(ServiceProvider);
            }
        }

        /// <summary>
        /// Cancel all inline edit items
        /// </summary>
        public void CancelAll()
        {
            editStates.Clear();
        }

        /// <summary>
        /// Edit item
        /// </summary>
        /// <param name="itm"></param>
        public void Edit(T itm)
        {
            var key = GetOpt.State.GetKey(itm);

            if (editStates.ContainsKey(key)) return;

            var model = GetOpt.InlineEdit.GetModel(itm);
            var itemState = new EditItemState<T>()
            {
                Item = itm,
                Key = key,
                Input = model,
                EditContext = new EditContext(model)
            };

            initItem(itemState);

            editStates.Add(key, itemState);

            SetFocusFirst(key);
        }

        private void SetFocusFirst(string key)
        {
            GetOpt.State.Component.AddPostRenderAction(async () =>
            {
                await GetOpt.State.Component.JS.InvokeVoidAsync(CompUtil.AweJs("inlfcs"), new { gdiv = GetOpt.State.Component.Div, key });
            });
        }

        /// <summary>
        /// Cancel inline edit item
        /// </summary>        
        public void Cancel(EditItemState<T> cx)
        {
            editStates.Remove(cx.Key);
        }

        /// <summary>
        /// Save inline edit item
        /// </summary>
        public async Task SaveAsync(EditItemState<T> cx)
        {
            var saveFunc = GetOpt.InlineEdit.Save;
            if (saveFunc is null)
            {
                if (GetOpt.InlineEdit.SaveAll is null)
                {
                    throw new AwesomeException($"GridOpt {nameof(GetOpt.InlineEdit.Save)} or {nameof(GetOpt.InlineEdit.SaveAll)} needs to be set for the save to work");
                }

                saveFunc = async cxs =>
                {
                    var res = await GetOpt.InlineEdit.SaveAll(new[] { cx });
                    return res.Count() == 1;
                };
            }

            if (await saveFunc(cx))
            {
                editStates.Remove(cx.Key);
            }

            await GetOpt.State.LoadAsync(new() { Partial = true });
        }

        /// <summary>
        /// Save all inline edit items
        /// </summary>
        public async Task SaveAllAsync()
        {
            var res = await GetOpt.InlineEdit.SaveAll(editStates.Values);
            if (editStates.Count > res.Count())
            {
                string key = editStates.First(st => !res.Contains(st.Key)).Key;
                SetFocusFirst(key);
            }

            foreach (var item in res)
            {
                editStates.Remove(item);
            }

            await GetOpt.State.LoadAsync(new() { Partial = true });
        }

        private Dictionary<string, EditItemState<T>> editStates = new();

        /// <summary>
        /// Get item state by key
        /// </summary>
        public EditItemState<T> GetItemState(string key)
        {
            return editStates.ContainsKey(key) ? editStates[key] : null;
        }

        /// <summary>
        /// Get create inline edit states 
        /// </summary>
        public EditItemState<T>[] GetCreateStates()
        {
            return editStates.Values.Where(editState => editState.IsCreate).ToArray();
        }
    }
}