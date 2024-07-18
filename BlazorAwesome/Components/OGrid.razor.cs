using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Omu.BlazorAwesome.Core;
using Omu.BlazorAwesome.Models;
using Omu.BlazorAwesome.Models.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Omu.BlazorAwesome.Components
{
    /// <summary>
    /// Awesome Grid Component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class OGrid<T>
    {
        private string aweid;
        private bool isReset;

        private ElementReference gdiv;
        private ElementReference cont;
        private OLoading loadingAnim;
        private ElementReference header;

        /// <summary>
        /// Grid State
        /// </summary>
        public GridState<T> State { get; private set; }

        /// <summary>
        /// Grid's main div
        /// </summary>
        public ElementReference Div => gdiv;

        private DotNetObjectReference<OGrid<T>> objRef;

        private async Task pageClick(int page)
        {
            if (State.IsLoading) return;
            State.Page = page;
            gridStateChanged();
            await State.LoadAsync();
            StateHasChanged();
        }

        private void gridStateChanged()
        {
            State.GridStateChanged();
        }

        private DropdownOpt filterDddOpt = new DropdownOpt
        {
            ClearBtn = true
        };

        //private GridOpt<T> opt;

        /// <summary>
        /// Grid options
        /// </summary>
        [Parameter]
        public GridOpt<T> Opt { get; set; }

        [Inject]
        internal IServiceProvider ServiceProvider { get; set; }

        private async Task onVisibleColumnChange(IEnumerable<string> value)
        {
            var nVisIds = value ?? new List<string>();
            var changed = false;
            var filterRemoved = false;
            foreach (var cs in State.ColumnsStates.Where(o => !o.Column.NoHide))
            {
                var isColHidden = !nVisIds.Contains(cs.Id);

                if (cs.Hidden == isColHidden) continue;

                cs.Hidden = isColHidden;
                changed = true;

                if (isColHidden && State.ClearFilterValsFor(cs.Id))
                {
                    filterRemoved = true;
                }
            }

            if (changed)
            {
                gridStateChanged();
            }

            if (filterRemoved)
            {
                await State.LoadAsync();
            }
        }

        private IEnumerable<string> visibleColsIds
        {
            get
            {
                var res = new List<string>();
                foreach (var cs in State.ColumnsStates.Where(o => !o.Column.NoHide))
                {
                    if (cs.Hidden) continue;
                    res.Add(cs.Id);
                }

                return res;
            }
        }

        private IEnumerable<KeyContent> pageSizeData()
        {
            var pageSizes = new List<int> { 5, 10, 20, 50, 100 };
            if (!pageSizes.Contains(Opt.PageSize))
            {
                pageSizes.Add(Opt.PageSize);
            }

            return pageSizes.OrderBy(o => o).Select(o => new KeyContent(o, o));
        }

        private IEnumerable<KeyContent> columnsSelectorData()
        {
            var res = new List<KeyContent>();

            foreach (var cs in State.ColumnsStates.Where(o => !o.Column.NoHide))
            {
                res.Add(new KeyContent(cs.Id, cs.Column.Label ?? cs.Column.Header ?? string.Empty));
            }

            return res;
        }

        private async Task onPageSizeChanged(int val)
        {
            State.PageSize = val;
            gridStateChanged();
            await State.LoadAsync();
        }

        private async Task remGroup(ColumnState<T> cs)
        {
            if (State.RemGroup(cs))
            {
                gridStateChanged();
                await State.LoadAsync();
            }
        }

        private async Task order(ColumnState<T> cs)
        {
            if (cs.Column.Bind is null || cs.Column.Sortable == false) return;
            if (State.IsLoading) return;

            State.OrderByColumn(cs);
            gridStateChanged();
            await State.LoadAsync();
        }

        private async Task reset()
        {
            State.Reset();
            isReset = true;
            gridStateChanged();
            await State.LoadAsync();
        }

        /// <summary>
        /// render
        /// </summary>
        public void Render()
        {
            StateHasChanged();
        }

        /// <summary>
        /// group by visible column index
        /// </summary>
        /// <param name="i">visible column index</param>
        /// <returns></returns>
        [JSInvokable]
        public async Task Group(int i)
        {
            if (State.Group(State.VisibleColumns.ElementAt(i)))
            {
                gridStateChanged();
                await State.LoadAsync();
                StateHasChanged();
            }
        }

        /// <summary>
        /// Save item by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>returns true on success</returns>
        [JSInvokable]
        public async Task<bool> Save(string key)
        {
            return await State.InlineEdit.SaveAsync(key);
        }

        /// <summary>
        /// reorder columns
        /// </summary>
        /// <param name="currenti">current index</param>
        /// <param name="hoveri">hovered index</param>
        [JSInvokable]
        public void Reorder(int currenti, int hoveri)
        {
            State.ReorderColumns(currenti, hoveri);
            gridStateChanged();
            StateHasChanged();
        }

        /// <summary>
        /// Row click
        /// </summary>
        /// <param name="k"></param>
        [JSInvokable]
        public void RowClick(string k)
        {
            Opt.RowClickFunc(k);
        }

        /// <summary>
        /// Row inline edit
        /// </summary>
        [JSInvokable]
        public async Task InlineEditRow(string k, int? cellIndex)
        {
            var rowModel = Opt.State.Items.Where(o => Opt.State.GetKey(o) == k).FirstOrDefault();
            
            if (rowModel is not null)
            {
                await Opt.State.InlineEdit.EditPrm(new () { Item = rowModel, CellIndexToFocus = cellIndex });
                gridStateChanged();
                StateHasChanged();
            }
        }

        /// <summary>
        /// Persist columns
        /// </summary>
        /// <param name="columns"></param>
        [JSInvokable]
        public void Persist(ClientColumn[] columns)
        {
            for (var i = 0; i < columns.Length; i++)
            {
                var col = columns[i];
                State.ColumnsStates[i].Width = col.W;
            }

            State.GridStateChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        public IJSRuntime JS => js;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                objRef = DotNetObjectReference.Create(this);
                var columns = getClientColumns();




                aweid = await js.InvokeAsync<string>(CompUtil.AweJs("regGGO"),
                new
                {
                    gdiv = gdiv,
                    objRef,
                    cont,
                    header,
                    h = Opt.Height,
                    conth = Opt.ContentHeight,
                    cw = Opt.ColumnWidth,
                    columns,
                    th = 0,
                    gl = State.Groups?.Length,
                    onRowClick = Opt.RowClickFunc is not null,
                    fzl = Opt.FrozenColumnsLeft,
                    fzr = Opt.FrozenColumnsRight,

                    // inline editing
                    rowClickEdit = Opt.InlineEdit?.RowClickEdit
                });

                return;
            }

            if (aweid is null) return;

            if (isReset)
            {
                isReset = false;
            }

            await js.InvokeVoidAsync(CompUtil.AweJs("gmod"), new { aweid, columns = getClientColumns() });
            var acts = postRenderActions;

            if (acts is not null)
            {
                postRenderActions = null;

                foreach (var act in acts)
                {
                    await act();
                }
            }
        }

        private List<Func<Task>> postRenderActions;

        /// <summary>
        /// add action to execute after render
        /// </summary>
        /// <param name="action"></param>
        public void AddPostRenderAction(Func<Task> action)
        {
            if (postRenderActions is null) postRenderActions = new();
            postRenderActions.Add(action);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void OnParametersSet()
        {
            setupState();

            if (Opt.State is null && State is not null)
            {
                Opt.State = State;
            }
            base.OnParametersSet();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            setupState();
            State.Log += "onInit, state setup; ";

            // load initial data
            if (Opt.Load)
            {
                await State.LoadAsync();
            }

            await base.OnInitializedAsync();
        }

        private void setupState()
        {
            if (State is not null)
            {
                return;
            }

            if (Opt is null)
            {
                throw new NullReferenceException("Grid Opt not set, Opt needs to be set before Grid's OnInitialized occurs");
            }

            if (Opt.Columns is null)
            {
                throw new NullReferenceException("Grid Opt.Columns not set, Opt.Columns needs to be set before Grid's OnInitialized occurs");
            }

            State = new();
            State.GetOpt = () => this.Opt;
            State.Component = this;
            State.Init((visible) => loadingAnim?.Render(visible), this);

            Opt.State = State;
        }

        private IEnumerable<ClientColumn> getClientColumns()
        {
            var columns = State.ColumnsStates
                .Where(c => !c.Hidden)
                .Select(cs => new ClientColumn
                {
                    Id = cs.Id,
                    W = cs.Width,
                    Grow = cs.Column.Grow,
                    Mw = CompUtil.GetMinWidth(cs.Column),
                    R = cs.Column.Resizable ?? true
                });

            return columns;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            if (objRef is not null)
            {
                objRef.Dispose();
                await CompUtil.TryDestroy(JS, gdiv);
            }
        }

        private string pageInfo()
        {
            var maxPageItems = State.PageSize * State.Page;
            var to = Math.Min(maxPageItems, State.Model.Count);
            var from = maxPageItems - (State.PageSize - 1);
            return string.Format("{0} - {1} ", from, to)
                + string.Format(OLangDict.Get(ODictKey.PageInfo), State.Model.Count);
        }

        private string getGridContentMinWidth()
        {
            if (State?.ColumnsStates is null) return null;

            var res = 0;

            foreach (var cs in State.ColumnsStates)
            {
                var w = cs.Width;
                if (w == 0)
                {
                    w = Opt.ColumnWidth;

                    if (cs.Column.MinWidth > 0)
                    {
                        w = cs.Column.MinWidth;
                    }
                }

                res += w;
            }

            return "min-width:" + res + "px";
        }
    }
}