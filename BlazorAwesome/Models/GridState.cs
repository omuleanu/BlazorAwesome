using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Omu.BlazorAwesome.Components;
using Omu.BlazorAwesome.Core;
using Omu.BlazorAwesome.Models.Utils;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid state model
    /// </summary>    
    public class GridState<T>
    {
        internal string Log { get; set; } = string.Empty;

        /// <summary>
        /// Grid Component
        /// </summary>
        public OGrid<T> Component { get; set; }

        /// <summary>        
        /// </summary>
        public Func<GridOpt<T>> GetOpt { private get; set; }

        /// <summary>
        /// Grid is loading data
        /// </summary>
        public bool IsLoading { get; set; } = true;

        private GridOpt<T> gopt => GetOpt();

        // required
        /// <summary>
        /// Columns states
        /// </summary>
        public IList<ColumnState<T>> ColumnsStates { get; set; }

        /// <summary>
        /// Grid render model
        /// </summary>
        public GridModel<T> Model { get; private set; } = new GridModel<T> { };

        /// <summary>
        /// Columns states that have grouping applied
        /// </summary>
        public ColumnState<T>[] Groups => ColumnsStates.Where(o => o.Sort != Sort.None && o.Group).OrderBy(o => o.Rank).ToArray();

        /// <summary>
        /// Grid items (page items)
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Total items count (for all pages)
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// Start/stop loading animation func
        /// </summary>
        public Action<bool> AnimateLoading { private get; set; }

        /// <summary>
        /// Filter data, used by filter controls
        /// </summary>
        public Dictionary<string, object> FilterData { get; set; } = new();

        /// <summary>
        /// Filter values, for each filter control
        /// </summary>
        public Dictionary<string, object> FilterValues { get; set; } = new();

        /// <summary>
        /// Collapsed headers data (GroupView key, (is collapsed, groupedColumns[]))
        /// </summary>
        public Dictionary<string, Tuple<bool, IEnumerable<string>>> CollapsedHeaders { get; set; } = new();

        /// <summary>
        /// Call GridOpt StateChange function if present
        /// </summary>
        public void GridStateChanged()
        {
            if (gopt is not null && gopt.StateChangeFunc is not null)
            {
                gopt.StateChangeFunc();
            }
        }

        /// <summary>
        /// Get key for item
        /// </summary>
        public string GetKey(T itm)
        {
            if (gopt.GetKeyFunc is not null)
            {
                return gopt.GetKeyFunc(itm).ToString();
            }

            if (gopt.Key is not null)
            {
                return GetColumnStrVal(gopt.Key, itm, "$$");
            }

            throw new AwesomeException("Grid Key not set, either GetKeyFunc, KeyProp or Key needs to be set");
        }

        /// <summary>
        /// Get column string value for a given itm and bind parameters
        /// </summary>        
        public string GetColumnStrVal(string bind, T itm, string separator = " ")
        {
            return CompUtil.ColumnBindValsToStr(GetBindValue(bind, itm), separator);
        }

        /// <summary>
        /// Get column value
        /// </summary>
        public IEnumerable<object> GetBindValue(string bind, object itm)
        {
            if (gopt.GetBindValue is not null)
            {
                return gopt.GetBindValue(bind, itm);
            }

            return CompUtil.GetColumnValue(bind, itm);
        }

        /// <summary>
        /// Inline edit state
        /// </summary>
        public InlineEditState<T> InlineEdit { get; private set; }

        /// <summary>        
        /// </summary>
        public bool HasNesting => nesting is not null;

        private NestingState<T> nesting;

        /// <summary>
        /// Nesting state
        /// </summary>
        public NestingState<T> Nesting
        {
            get
            {
                if (nesting is null)
                {
                    nesting = new(this);
                }

                return nesting;
            }
        }

        //public OGrid<T> Grid { get; set; }

        /// <summary>
        /// Init grid state
        /// </summary>
        public void Init(Action<bool> animateLoading, OGrid<T> grid)
        {
            Page = gopt.Page;
            PageSize = gopt.PageSize;

            //Grid = grid;
            //Render = render;

            Load = gopt.Load;
            AnimateLoading = animateLoading;
            IsLoading = gopt.Load;

            CompUtil.SetColumnsDefaults(gopt, gopt.Columns);

            ColumnsStates = gopt.Columns.Select(c => new ColumnState<T>()
            {
                Column = c,
                Id = c.Id,
            }).ToList();

            resetColumns();

            if (gopt.InlineEdit is not null)
            {
                InlineEdit = new()
                {
                    GetOpt = gopt,
                    ServiceProvider = Component.ServiceProvider
                };
            }
        }

        /// <summary>
        /// Load on init
        /// </summary>
        public bool Load { get; set; }

        /// <summary>
        /// Load options
        /// </summary>
        public LoadOpt LoadOpt { get; private set; }

        /// <summary>
        /// Load grid data and render
        /// </summary>
        public async Task LoadAsync(LoadOpt lopt = null)
        {
            Log += "start load;";
            LoadOpt = lopt;
            bool stateChanged = false;

            void unsetSortGroupForHidColumns()
            {
                foreach (var col in gopt.Columns)
                {
                    if (col.Hidden)
                    {
                        col.Sort = Sort.None;
                        col.Group = false;
                    }
                }
            }

            try
            {
                IsLoading = true;

                if (Load == false)
                {
                    Load = true;
                    Render();
                }

                if (gopt.BeginLoadFunc is not null) gopt.BeginLoadFunc();

                if (!(lopt?.Partial == true))
                {
                    Nesting?.OpenedNests.Clear();
                    InlineEdit?.CancelAll();
                }

                if (Items is not null)
                {
                    AnimateLoading(true);
                }

                unsetSortGroupForHidColumns();

                foreach (var cs in ColumnsStates.Where(o => o.Hidden))
                {
                    this.ClearFilterValsFor(cs.Id);
                    stateChanged = true;
                }

                resetRank();

                if (lopt != null && lopt.LazyItem != null && gopt.LoadLazyNodeAsync != null)
                {
                    await gopt.LoadLazyNodeAsync((T)lopt.LazyItem);
                }

                FilterData.Clear();
                if (Log.Length > 100) Log = string.Empty;
                var sw = new Stopwatch();
                sw.Start();
                if (gopt.LoadData is not null)
                {
                    await gopt.LoadData();
                }
                else
                {
                    var query = gopt.GetQuery();
                    if (query == null)
                    {
                        throw new ArgumentNullException(nameof(gopt.GetQuery),
                            "Either GridOpt LoadData or GetQuery properties need to be set");
                    }

                    query = await this.ApplyFilters(query);

                    this.SetCount(query.LongCount());

                    Count = query.LongCount();
                    Items = this.QueryPage(query).ToArray();
                }
                sw.Stop();
                Log += " load: " + sw.Elapsed;
            }
            finally
            {
                IsLoading = false;
                AnimateLoading(false);
                LoadOpt = null;
            }

            buildModel();

            // not necessary for inline edit create, will see later for others
            // necessary for tree grid lazy loading load
            Render();
            Log += "end load/render;";

            // when removing filters for hidden columns
            if (stateChanged)
            {
                GridStateChanged();
            }

            if (gopt.LoadFunc is not null) gopt.LoadFunc();
        }

        /// <summary>
        /// Render the grid component
        /// </summary>
        public Action Render => Component.Render;

        // sorting order
        private int rank;

        private void resetRank()
        {
            var scols = CompUtil.GetSortColumns(this);

            var colRank = 0;
            foreach (var col in scols)
            {
                col.Rank = colRank++;
            }

            rank = colRank;
        }

        /// <summary>
        /// Get current grid state sorting rules
        /// </summary>
        public IDictionary<string, Sort> GetSortRules()
        {
            var scols = CompUtil.GetSortColumns(this);

            var dict = scols.ToDictionary(o => o.Column.Bind, o => o.Sort);

            if (dict.Count == 0 && gopt.Key != null && gopt.DefaultKeySort != Sort.None)
            {
                dict.Add(gopt.Key, gopt.DefaultKeySort);
            }

            return dict;
        }

        /// <summary>
        /// Apply current sorting rules to query
        /// </summary>
        public IQueryable<T> OrderBy(IQueryable<T> query)
        {
            var dict = GetSortRules();

            if (gopt.OrderBy is not null)
            {
                return gopt.OrderBy(query);
            }

            return Dlinq.OrderBy(query, dict);
        }

        private GridModel<T> buildModel()
        {
            maxDepth = 0;
            if (IsLoading) return Model;

            var res = new GridModel<T>();

            if (Items != null)
            {
                res.Data = buildGroupView(Items.ToArray(), 0, string.Empty, Enumerable.Empty<string>());
                res.PageCount = AweUtil.GetPageCount(Count, PageSize);
                res.Count = Count;
                res.GroupsCount = Groups.Count();
            }

            res.TreeHeight = maxDepth;

            Model = res;

            return res;
        }

        private int maxDepth;

        private void setDepth(int depth)
        {
            if (depth > maxDepth) maxDepth = depth;
        }

        private string GetGroupKey(string prefix, ColumnState<T> cs, GroupInfo<T> info, IEnumerable<T> items)
        {
            var val = GetColumnStrVal(info.Column.Bind, items.First());
            return prefix + "$" + cs.Column.Id + "=" + val;
        }

        private GroupView<T> buildGroupView(
            T[] items,
            int groupIndex,
            string parentGroupKey,
            IEnumerable<string> parentGroups
            )
        {
            var result = new GroupView<T>();

            if (groupIndex == Groups.Length)
            {
                if (gopt.GetChildren is null || !items.Any())
                {
                    setDepth(groupIndex);

                    result.Depth = groupIndex;
                    result.Items = items;
                }
                else
                {
                    buildNode(result, items, groupIndex, 0);
                }

                return result;
            }

            var gviews = new List<GroupView<T>>();
            var gitems = new List<T>();
            var gcolState = Groups[groupIndex];

            T prev = default;

            void addGroup()
            {
                var ginfo = new GroupInfo<T>
                {
                    Column = gcolState.Column,
                    Items = gitems
                };

                var groupCol = gcolState.Column.Id;

                var currentGroups = parentGroups.Concat(new[] { groupCol });

                var gkey = GetGroupKey(parentGroupKey, gcolState, ginfo, gitems);
                var gv = buildGroupView(gitems.ToArray(), groupIndex + 1, gkey, currentGroups);

                gv.Header = getMakeHeader()(ginfo);
                gv.Key = gkey;
                gv.GroupColumns = currentGroups;

                if (gopt.MakeFooter is not null)
                {
                    gv.Footer = gopt.MakeFooter(ginfo);
                }

                gviews.Add(gv);
                gitems.Clear();
            }

            for (var i = 0; i < items.Length; i++)
            {
                if (prev != null && !CompUtil.AreInSameGroup(this, gcolState.Column.Bind, prev, items[i]))
                {
                    addGroup();
                }

                prev = items[i];
                gitems.Add(prev);

                if (i == items.Length - 1)
                {
                    addGroup();
                }
            }

            result.Groups = gviews;
            var depth = groupIndex;
            setDepth(depth);
            result.Depth = depth;

            return result;
        }

        private void buildNode(GroupView<T> result, IEnumerable<T> groupItems, int depth, int nodeLevel)
        {
            if (depth > maxDepth) maxDepth = depth;

            var groups = new List<GroupView<T>>();

            foreach (var groupItem in groupItems)
            {
                var children = gopt.GetChildren(groupItem, nodeLevel + 1) ?? Enumerable.Empty<T>().AsQueryable();

                var list = children as IEnumerable<T>;
                var lazyNode = ReferenceEquals(children, Awe.LazyNode);

                if (list is null && !lazyNode)
                {
                    throw new ArgumentException("GetChildren should return an IQueryable<T> or Awe.LazyNode");
                }

                //if (list != null)
                //{
                //    list = OrderBy(list)
                //    if (orderParms != null)
                //    {
                //        list = OrderByFunc(list, orderParms.ToArray());
                //    }
                //}

                if (lazyNode || list.Any())
                {
                    var isLazy = lazyNode;

                    var nodeGroup = new GroupView<T>();
                    nodeGroup.NodeType = lazyNode ? NodeType.Lazy : NodeType.Node;
                    nodeGroup.Depth = depth;
                    nodeGroup.Key = GetKey(groupItem).ToString();
                    nodeGroup.Header =
                        getMakeHeader()(
                            new GroupInfo<T>
                            {
                                //AllItems = allItems,
                                Items = list,
                                NodeItem = groupItem,
                                Level = depth,
                                NodeLevel = nodeLevel,
                                Lazy = isLazy,
                            });

                    if (isLazy)
                    {
                        nodeGroup.Header.Collapsed = true;
                    }
                    else
                    {
                        buildNode(nodeGroup, list, depth + 1, nodeLevel + 1);
                    }

                    groups.Add(nodeGroup);
                }
                else
                {
                    // leaf
                    if (groups.Count == 0 || groups.Last().NodeType != NodeType.Items)
                    {
                        groups.Add(new GroupView<T>
                        {
                            Items = new T[] { groupItem },
                            NodeType = NodeType.Items,
                            Depth = depth
                        });
                    }
                    else
                    {
                        var last = groups.Last();
                        last.Items = last.Items.Concat(new[] { groupItem });
                    }
                }
            }

            result.Groups = groups;
        }

        private Func<GroupInfo<T>, GroupHeader<T>> getMakeHeader()
        {
            GroupHeader<T> makeHeader(GroupInfo<T> info)
            {
                if (info.NodeItem != null)
                {
                    return new GroupHeader<T>
                    {
                        Item = info.NodeItem
                    };
                }

                var val = GetColumnStrVal(info.Column.Bind, info.Items.First());

                return new GroupHeader<T>
                {
                    Content = info.Column.Header + ": " + CompUtil.HtmlEncode(val),
                    Collapsed = false
                };
            }

            return gopt.MakeHeader ?? makeHeader;
        }

        /// <summary>
        /// reset page, filtering, columns, collapsed headers
        /// </summary>
        public void Reset()
        {
            CollapsedHeaders.Clear();
            FilterValues.Clear();
            resetColumns();
            Page = gopt.Page;
            //PageSize = gopt.PageSize;

            var res = new List<ColumnState<T>>();

            foreach (var c in gopt.Columns)
            {
                res.Add(ColumnsStates.Single(o => o.Column == c));
            }

            ColumnsStates.Clear();
            ColumnsStates.AddArray(res);
        }

        private void resetColumns()
        {
            foreach (var cs in ColumnsStates)
            {
                cs.Hidden = cs.Column.Hidden;
                cs.Width = cs.Column.Width;
                cs.Sort = cs.Column.Sort;
                cs.Group = cs.Column.Group;
                cs.Rank = cs.Column.Rank;
            }
        }

        /// <summary>
        /// Page
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; } = 10;

        internal void OrderByColumn(ColumnState<T> col)
        {
            if (col.Sort == Sort.None)
            {
                col.Sort = Sort.Asc;
                col.Rank = rank++;
            }
            else if (col.Sort == Sort.Asc)
            {
                col.Sort = Sort.Desc;
            }
            else
            {
                if (col.Group)
                {
                    col.Sort = Sort.Asc;
                }
                else
                {
                    col.Sort = Sort.None;
                    col.Rank = null;
                }
            }
        }

        internal void ReorderColumns(int fromi, int toi)
        {
            var fcol = ColumnsStates[fromi];
            ColumnsStates.RemoveAt(fromi);
            ColumnsStates.Insert(toi, fcol);
        }

        #region grouping
        /// <summary>
        /// Remove group
        /// </summary>
        public bool RemGroup(ColumnState<T> columnState)
        {
            if (!columnState.Group)
            {
                return false;
            }

            columnState.Group = false;
            columnState.Sort = Sort.None;

            // remove collapsed headers with the same group
            var groupName = columnState.Column.Id;
            foreach (var keyValue in CollapsedHeaders)
            {
                if (!keyValue.Value.Item2.Contains(groupName))
                {
                    continue;
                }

                CollapsedHeaders.Remove(keyValue.Key);
            }

            return true;
        }

        /// <summary>
        /// Group by column
        /// </summary>
        public bool Group(ColumnState<T> columnState)
        {
            var bind = columnState.Column.Bind;
            if (bind == null || columnState.Group) return false;

            if (ColumnsStates.Any(o => o.Group && o.Column.Bind == bind))
            {
                return false;
            }

            columnState.Rank = rank++;
            if (columnState.Sort == Sort.None)
            {
                columnState.Sort = Sort.Asc;
            }

            columnState.Group = true;
            return true;
        }

        #endregion

        /// <summary>        
        /// </summary>
        public IEnumerable<ColumnState<T>> VisibleColumns
        {
            get { return ColumnsStates.Where(o => !o.Hidden); }
        }
    }
}