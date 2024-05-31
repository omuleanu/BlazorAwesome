using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Omu.BlazorAwesome.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Omu.BlazorAwesome.Core;
using Omu.BlazorAwesome.Models.Utils;
using System.ComponentModel;

namespace Omu.BlazorAwesome.Components.Internal
{
    /// <summary>
    /// ODropdown base
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public partial class ODropdown<TKey>
    {
        private DotNetObjectReference<ODropdown<TKey>> objRef;

        private int timesRend;
#pragma warning disable 649
        private ElementReference dropdownField;
        private ElementReference btn;
        private ElementReference txtSearch;
        private ElementReference txtSearch2;
        private OLoadingMini loadingMiniInField;
        private OLoadingMini loadingMiniInPopup;
        private ElementReference multiCont;
        private OPopup popup;
        private OSList<TKey> selectList;
        private Func<PopupOpt> ddpopupOpt;

#pragma warning restore 649                

        /// <summary>
        /// Editor state
        /// </summary>
        public EditorState State { get; private set; }

        private string pleaseSelect = OLangDict.Get(ODictKey.PleaseSelect);

        /// <summary>        
        /// </summary>
        protected bool HasClearBtn => CompUtil.HasClearButton(Opt, OContext);

        private string parentValue;

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
        /// Get selected items
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyContent> GetSelectedItems()
        {
            var values = GetValue();

            var selItems = new List<KeyContent>();

            var data = getData();

            if (data is null)
            {
                return selItems;
            }

            foreach (var v in values)
            {
                var item = data.FirstOrDefault(o => object.Equals(o.Key, v));

                if (item != null)
                {
                    selItems.Add(item);
                }
            }

            return selItems;
        }

        /// <summary>
        /// get data
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<KeyContent> getData()
        {
            return State?.Data ?? Opt?.Data ?? prmData;
        }

        /// <summary>
        /// check if value is present in data, clear if not present (unless combo)
        /// </summary>
        /// <returns></returns>
        protected async Task checkValInData()
        {
            if (combo) return;

            await CompUtil.CheckValues(values, getData(), async (nvals) =>
            {
                values.Clear();
                values.AddRange(nvals);
                await ValueChangedInvoke();
            });
        }

        //async ValueTask IAsyncDisposable.DisposeAsync()
        //{
        //    await Close();
        //}

        // reload when Data or DataFunc have changed
        //private bool loadOnParameterSet;

        /// <summary>
        /// Options
        /// </summary>
        [Parameter]
        public DropdownOpt Opt { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>        
        protected override async Task OnParametersSetAsync()
        {
            //await checkValInData(); // infinite loop when value not contained in data

            await parentReload();
        }

        /// <summary>
        /// Calls State.Load if parent value changed
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

        private IEnumerable<KeyContent> prmData;

        /// <summary> 
        /// Sets state or prm data
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
        }

        private string comboSearchVal
        {
            get
            {
                if (values is not { Count: 1 }) return string.Empty;

                var v0 = values[0];
                var item = getData().FirstOrDefault(o => Equals(o.Key, v0));

                return item is null ? v0.ToString() : item.Content;
            }
        }

        private void onComboInput(ChangeEventArgs args)
        {
            var value = args.Value?.ToString();

            var itmByContent = getData().FirstOrDefault(o => o.Content == value);
            values.Clear();

            if (itmByContent != null)
            {
                values.Add(itmByContent.Key);
            }
            else
            {
                // combo value
                values.Add(value);
            }

            // set combo value and filter for selectlist
            if (selectList != null && selectList.FilterStr != value)
            {
                selectList.FilterStr = value;
                StateHasChanged();

                //selectList.SetValue(values); // avoid render field
                //selectList.Render(); 
            }
        }

        private async Task onComboChange(ChangeEventArgs args)
        {
            onComboInput(args);

            await ValueChangedInvoke();
        }

        private async Task comboTxtSearchKeyDown(KeyboardEventArgs args)
        {
            var key = args.Key;
            if (key == AweKey.Backspace
                || key.Length == 1 // a b c ...
                || key == AweKey.ArrowUp
                || key == AweKey.ArrowDown)
            {
                openPopup();
            }

            if (key == AweKey.Escape)
            {
                await popup.CloseAsync();
            }
        }

        private async Task removeValueBtn(TKey v)
        {
            if (IsDisabled) return;

            var sel = values.FirstOrDefault(o => o.Equals(v));

            var i = values.IndexOf(sel);
            if (i > -1)
            {
                values.RemoveAt(i);
            }

            await popup.CloseAsync();

            if (!isMobile)
            {
                await txtSearch.FocusAsync();
            }

            await ValueChangedInvoke();
        }

        private string FilterStr
        {
            get => selectList is null || !popup.IsOpen ? string.Empty : selectList.FilterStr;

            set
            {
                selectList.FilterStr = value;
            }
        }

        private void openPopup()
        {
            if (popup.IsOpen) return;

            var prm = new PopupOpenPrm { Opener = dropdownField }; //, NoFocus = multiselect || Combo

            popup.Open(prm);
        }

        private async Task openBtnClick()
        {
            if (IsDisabled) return;

            openPopup();

            if ((multiselect || combo) && !isMobile)
            {
                await txtSearch.FocusAsync();
            }

            //shouldRender = false;
        }

        private async Task multiTxtSearchKeyDown(KeyboardEventArgs args)
        {
            var isBackspace = args.Key == AweKey.Backspace;

            // remove sel item via backspace
            if (FilterStr.Length == 0 && isBackspace && values.Any())
            {
                values.RemoveAt(values.Count - 1);
                //focusSearchTxtAfterRender = true;
                StateHasChanged();
                await ValueChangedInvoke();
                await popup.CloseAsync();
                return;
            }

            if (args.Key == AweKey.Escape)
            {
                await popup.CloseAsync();
            }
            else if (!AweKey.NonOpenKeys.Contains(args.Key))
            {
                openPopup();
            }
        }

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            ddpopupOpt = () => new()
            {
                Dropdown = true,
                PopupClass = "o-menu open",
                Menu = true,
                OutClickClose = true,
                NoCloseFocus = multiselect,
                NoFocus = combo || multiselect //isMobile || called isMobile() in js
            };

            setupState();

            await State?.LoadAsync();

            await checkValInData();
        }

        /// <summary>
        /// is multiple
        /// </summary>
        protected bool multiple;

        /// <summary>
        /// is multiselect
        /// </summary>
        protected bool multiselect;

        /// <summary>
        /// main div css class
        /// </summary>
        protected override string fieldClass
        {
            get
            {
                var res = base.fieldClass;

                if (HasClearBtn)
                {
                    res += " o-hasclr";
                }

                return res;
            }
        }

        /// <summary>
        /// Component css class
        /// </summary>
        protected virtual string typeClass => string.Empty;

        private async Task onItemSelected(ItemSelectedPrm prm)
        {
            var closeOnSelect = Opt is null || !(Opt.NoSelectClose is true);

            if (multiple && !multiselect)
            {
                // multichk
                closeOnSelect = Opt is not null && Opt.NoSelectClose == false;
            }

            if (closeOnSelect)
            {
                await popup.CloseAsync();
            }

            if (prm.UserClick)
            {
                if (combo)
                {
                    await txtSearch.FocusAsync();
                }
                else if (!multiselect && closeOnSelect)
                {
                    await btn.FocusAsync();
                }
            }

            values.Clear();
            values.AddArray(prm.Value);

            await ValueChangedInvoke();
        }

        private async Task clearValue()
        {
            if (!values.Any()) return;
            values.Clear();
            await ValueChangedInvoke();
        }

        /// <summary>
        /// Move value
        /// </summary>
        /// <param name="from">from index</param>
        /// <param name="to">to index</param>
        /// <returns></returns>
        [JSInvokable]
        public async Task MoveVal(int from, int to)
        {
            if (values.Count < Math.Max(from, to) + 1) return;

            var item = values[from];
            values.RemoveAt(from);

            values.Insert(to, item);

            await ValueChangedInvoke();

            StateHasChanged();
        }

        /// <summary>
        /// Open dropdown
        /// </summary>
        /// <returns></returns>
        [JSInvokable]
        public ElementReference Open()
        {
            openPopup();
            return popup.PopupRef;
        }

        /// <summary>
        /// Close dropdown
        /// </summary>        
        [JSInvokable]
        public async Task Close()
        {
            await popup.CloseAsync();
        }

        /// <summary> 
        /// Function called by JS when user stops typing in the search box
        /// </summary>        
        [JSInvokable]
        public async Task Search(string value)
        {
            if (Opt is null || Opt.SearchFunc is null || State is null || getData() is null) return;

            var prevItemsCount = getData().Count();

            try
            {
                loadingMiniInField?.Render(true);
                loadingMiniInPopup?.Render(true);

                var dataChanged = await Opt.SearchFunc(value);
                if (dataChanged)
                {
                    StateHasChanged();
                }
            }
            finally
            {
                loadingMiniInField?.Render(false);
                loadingMiniInPopup?.Render(false);
            }
        }

        private DotNetObjectReference<ODropdown<TKey>> getObjRef()
        {
            if (objRef is null)
            {
                objRef = DotNetObjectReference.Create(this);
            }

            return objRef;
        }

        private async Task srcTxtAfterRender(bool firstRender)
        {
            if (!firstRender) return;
            if (Opt is null || Opt.SearchFunc is null) return;

            await JSUtil.InvokeVoidAsync(JS, CompUtil.AweJs("onStopInp"), new { inp = txtSearch, inp2 = txtSearch2, objRef = getObjRef() });
        }

        /// <summary>        
        /// <inheritdoc/>
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !IsDisabled)
            {
                if (multiselect)
                {
                    // apply caption, drag and drop reorder items
                    await JSUtil.InvokeVoidAsync(JS, CompUtil.AweJs("multis"), new { cont = multiCont, objRef = getObjRef() });
                }

                if (multiselect || combo)
                {
                    await srcTxtAfterRender(firstRender);
                }

                if (Opt?.OpenOnHover is true)
                {
                    await JSUtil.InvokeVoidAsync(JS, CompUtil.AweJs("openOnHover"), new { cont = dropdownField, objRef = getObjRef() });
                }

                isMobile = await JS.InvokeAsync<bool>(CompUtil.AweJs("isMobile"));
            }

            if (multiselect && !values.Any())
            {
                // apply multiselect caption
                await JS.InvokeVoidAsync(CompUtil.AweJs("multis"), new { cont = multiCont, apl = 1 });
            }
        }
        private bool isMobile;
        /// <summary>
        /// Get Opt config model
        /// </summary>        
        protected override IEditorOpt getOpt()
        {
            return Opt;
        }

        /// <summary>
        /// Trigger after change, state change, field change
        /// </summary>        
        protected async Task triggerChange()
        {
            await afterChange();

            StateHasChanged();
            FieldChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            objRef?.Dispose();
        }
    }
}