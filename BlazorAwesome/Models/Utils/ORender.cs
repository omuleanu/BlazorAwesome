using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Omu.BlazorAwesome.Components;
using Omu.BlazorAwesome.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Omu.BlazorAwesome.Models.Utils
{
    /// <summary>
    /// Render util methods
    /// </summary>
    public static class ORender
    {
        /// <summary>
        /// Multiselect inline editor
        /// </summary>        
        public static Func<ColumnEditorContext<T>, RenderFragment> Multiselect<T>(IGridStateProp<T> gopt, InlDropdownPrm prm)
        {
            return InlMulti<T>(gopt, prm, typeof(OMultiselect<object>));
        }

        /// <summary>
        /// Multicheck inline editor
        /// </summary>
        public static Func<ColumnEditorContext<T>, RenderFragment> Multicheck<T>(IGridStateProp<T> gopt, InlDropdownPrm prm)
        {
            return InlMulti<T>(gopt, prm, typeof(OMulticheck<object>));
        }

        /// <summary>
        /// DatePicker inline editor
        /// </summary>
        public static Func<ColumnEditorContext<T>, RenderFragment> DatePicker<T>(IGridStateProp<T> gopt)
        {
            return (cx) =>
            {
                return builder =>
                {
                    var receiver = gopt.State.Component;
                    var state = cx.EditItemState;
                    var column = cx.Column;
                    var input = state.Input;

                    var propName = column.Bind;
                    DateTime? getVal(object inp) => (DateTime?)gopt.State.GetBindValue(propName, inp).First();
                    var prop = input.GetType().GetProperty(propName);

                    var objPropValExpr = Expression.Convert(Expression.Property(Expression.Constant(input), prop), typeof(object));
                    var propValExpr = Expression.Property(Expression.Constant(input), prop);

                    var fieldForLambda = Expression.Lambda<Func<object>>(objPropValExpr);

                    var valueExprLambda = Expression.Lambda<Func<DateTime?>>(propValExpr);

                    var propValParam = Expression.Parameter(typeof(DateTime?));
                    var assignPropValExpr = Expression.Assign(propValExpr, propValParam);
                    var assignLambda = Expression.Lambda<Action<DateTime?>>(assignPropValExpr, propValParam).Compile();

                    builder.OpenComponent<OFieldInl>(0);
                    builder.AddAttribute(1, "For", RuntimeHelpers.TypeCheck(fieldForLambda));
                    builder.AddAttribute(2, "ChildContent",
                        (RenderFragment)
                        (builder2 =>
                        {
                            builder2.OpenComponent<ODatePicker>(3);
                            builder2.AddAttribute(4, "Value", RuntimeHelpers.TypeCheck(getVal(input)));
                            builder2.AddAttribute(5,
                                "ValueChanged",
                                RuntimeHelpers.TypeCheck(EventCallback.Factory.Create(
                                    receiver,
                                    RuntimeHelpers.CreateInferredEventCallback(receiver, assignLambda, getVal(input))
                                    )));

                            builder2.AddAttribute(6, "ValueExpression", RuntimeHelpers.TypeCheck(valueExprLambda));

                            builder2.CloseComponent();
                        }));

                    builder.CloseComponent();
                };
            };
        }

        /// <summary>
        /// Toggle inline editor
        /// </summary>
        public static Func<ColumnEditorContext<T>, RenderFragment> Toggle<T>(IGridStateProp<T> gopt)
        {
            return InlChk<T, OToggle>(gopt);
        }

        /// <summary>
        /// Checkbox inline editor
        /// </summary>
        public static Func<ColumnEditorContext<T>, RenderFragment> Checkbox<T>(IGridStateProp<T> gopt)
        {
            return InlChk<T, OCheckbox>(gopt);
        }

        /// <summary>
        /// Textbox inline editor
        /// </summary>
        public static Func<ColumnEditorContext<T>, RenderFragment> TextBox<T>(
            IGridStateProp<T> gopt)
        {
            return (cx) =>
            {
                return builder =>
                {
                    var receiver = gopt.State.Component;
                    var state = cx.EditItemState;
                    var column = cx.Column;
                    var input = state.Input;

                    var propName = column.Bind;
                    string getVal(object inp) => string.Join(string.Empty, gopt.State.GetBindValue(propName, inp));
                    var prop = input.GetType().GetProperty(propName);
                    var inputPropValExpr = Expression.Property(Expression.Constant(input), prop);

                    var fieldForLambda = Expression.Lambda<Func<object>>(inputPropValExpr);

                    var valueExprLambda = Expression.Lambda<Func<string>>(inputPropValExpr);

                    var propValParam = Expression.Parameter(typeof(string));
                    var assignPropValExpr = Expression.Assign(inputPropValExpr, propValParam);
                    var assignLambda = Expression.Lambda<Action<string>>(assignPropValExpr, propValParam).Compile();

                    builder.OpenComponent<OFieldInl>(0);
                    builder.AddAttribute(1, "For", RuntimeHelpers.TypeCheck(fieldForLambda));
                    builder.AddAttribute(2, "ChildContent",
                        (RenderFragment)
                        (builder2 =>
                        {
                            builder2.OpenComponent<OTextbox>(3);
                            builder2.AddAttribute(4, "Value", RuntimeHelpers.TypeCheck(getVal(input).ToString()));
                            builder2.AddAttribute(5,
                                "ValueChanged",
                                RuntimeHelpers.TypeCheck(EventCallback.Factory.Create(
                                    receiver,
                                    RuntimeHelpers.CreateInferredEventCallback(receiver, assignLambda, getVal(input))
                                    )));

                            builder2.AddAttribute(6, "ValueExpression", RuntimeHelpers.TypeCheck(valueExprLambda));

                            builder2.CloseComponent();
                        }));

                    builder.CloseComponent();
                };
            };
        }

        /// <summary>
        /// Grid column with inline edit button
        /// </summary>
        public static Column<T> InlEditColumn<T>(ComponentBase receiver, IGridStateProp<T> gopt)
        {
            return new Column<T>()
            {
                Label = "edit action",
                Width = 100,
                Render = InlEditButton(gopt, receiver)
            }
            .Editor(InlSaveButton(gopt, receiver));
        }

        /// <summary>
        ///  Grid column with inline delete button
        /// </summary>
        public static Column<T> InlDeleteColumn<T>(ComponentBase receiver, IGridStateProp<T> gopt, Action<T> confirmDelete)
        {
            return new Column<T>()
            {
                Label = "delete action",
                Width = 100,
                Render = itm => Button(receiver, "Delete", () => confirmDelete(itm))
            }.Editor(cx => Button(receiver, "Cancel", () => gopt.State.InlineEdit.Cancel(cx.EditItemState)));
        }

        /// <summary>
        /// Grid inline create button
        /// </summary>
        public static RenderFragment InlCreateButton<T>(IGridStateProp<T> gopt, ComponentBase receiver, T item, Func<object> model)
        {
            if (item == null) return null;
            var itemKey = gopt.State.GetKey(item);
            return builder =>
            BuildButton(builder, null,
            EventCallback.Factory.Create<MouseEventArgs>(receiver, (args) => gopt.State.InlineEdit.Create(model(), itemKey)), "Add Child");
        }

        /// <summary>
        /// Grid inline edit button
        /// </summary>
        public static Func<T, RenderFragment> InlEditButton<T>(IGridStateProp<T> gopt, ComponentBase receiver)
        {
            return itm =>
            {
                return builder =>
                BuildButton(builder, null,
                EventCallback.Factory.Create<MouseEventArgs>(receiver, (args) => gopt.State.InlineEdit.Edit(itm)), "Edit");
            };
        }

        /// <summary>
        /// Grid inline save button
        /// </summary>
        public static Func<ColumnEditorContext<T>, RenderFragment> InlSaveButton<T>(IGridStateProp<T> gopt, ComponentBase receiver)
        {
            return (cx) =>
            {
                return builder =>
                    BuildButton(builder, null,
                    EventCallback.Factory.Create<MouseEventArgs>(receiver, (args) => gopt.State.InlineEdit.SaveAsync(cx.EditItemState)), "Save");
            };
        }

        /// <summary>
        /// Multiselect grid filter
        /// </summary>
        public static RenderFragment FilterMultiselect<T, TKey>(this GridState<T> state, ComponentBase receiver, ColumnState<T> cs)
        {
            var name = cs.Id;
            return builder =>
            {
                builder.OpenComponent<OMultiselect<TKey>>(0);
                builder.AddAttribute(1, "ValueChanged", EventCallback.Factory.Create<IEnumerable<TKey>>(receiver, v => state.FilterValChange(name, v)));
                builder.AddAttribute(2, "Value", state.GetFilterValue(name));
                builder.AddAttribute(3, "Opt", new DropdownOpt { ClearBtn = true, Data = (IEnumerable<KeyContent>)state.GetFilterData(name) });
                builder.CloseComponent();
            };
        }

        /// <summary>
        /// Textbox grid filter
        /// </summary>
        public static RenderFragment FilterTextbox<T>(this GridState<T> state, ComponentBase receiver, ColumnState<T> cs)
        {
            var name = cs.Id;
            return builder =>
            {
                builder.OpenComponent<OTextbox>(0);
                builder.AddAttribute(1, "ValueChanged", EventCallback.Factory.Create<string>(receiver, v => state.FilterValChange(name, v)));
                builder.AddAttribute(2, "Value", state.GetFilterValue(name));
                builder.AddAttribute(3, "Opt", new TextboxOpt { ClearBtn = true });
                builder.CloseComponent();
            };
        }

        /// <summary>
        /// Numeric grid filter, TValue = int?, Opt.Min = 0
        /// </summary>
        public static RenderFragment FilterNumeric<T>(this GridState<T> state, ComponentBase receiver, ColumnState<T> cs, NumericInputOpt<int?> opt = null)
        {
            return FilterNumeric<T, int?>(state, receiver, cs, opt ?? new() { Min = 0 });
        }

        /// <summary>
        /// Numeric grid filter
        /// </summary>
        public static RenderFragment FilterNumeric<TGridModel, TValue>(
            this GridState<TGridModel> state,
            ComponentBase receiver, ColumnState<TGridModel> cs,
            NumericInputOpt<TValue> opt = null,
            bool useOp = false)
        {
            var name = cs.Id;
            return builder =>
            {
                builder.OpenComponent<ONumericInput<TValue>>(0);
                builder.AddAttribute(1, "ValueChanged", EventCallback.Factory.Create<TValue>(receiver, v => state.FilterValChange(name, v)));

                var val = state.GetFilterValue(name);
                builder.AddAttribute(2, "Value", val);

                if(opt is null) opt = new();
                opt.CssClass = (opt.CssClass ?? "") + " o-first";

                if (opt != null)
                {
                    builder.AddAttribute(3, "Opt", opt);
                }

                builder.CloseComponent();

                if (useOp)
                {
                    var opName = name + "_Op";
                    var opval = "=";

                    BuildDateOpDropdownList(
                        builder,
                        "o-op",
                        EventCallback.Factory.Create<string>(receiver, v => state.FilterValChange(opName, v)),
                        (string)(state.GetFilterValue(opName) ?? opval));
                }

                BuildClearAllButton(builder, EventCallback.Factory.Create<MouseEventArgs>(receiver, (args) => state.ClearFilterValsForAndLoad(name)));

                //BuildButton(builder,
                //    "awe-clrbtn",
                //    EventCallback.Factory.Create<MouseEventArgs>(receiver, (args) => state.ClearFilterValsForAndLoad(name)),
                //    "<span class=\"awe-icon awe-icon-x\"></span>");
            };
        }

        /// <summary>
        /// DatePicker grid filter
        /// </summary>
        public static RenderFragment FilterDatePicker<T>(this GridState<T> state, ComponentBase receiver, string name)
        {
            return builder =>
            {
                builder.OpenComponent<ODatePicker>(0);
                builder.AddAttribute(1, "ValueChanged", EventCallback.Factory.Create<DateTime?>(receiver, v => state.FilterValChange(name, v)));
                builder.AddAttribute(2, "Value", state.GetFilterValue(name));
                builder.AddAttribute(3, "Opt", new DatePickerOpt { ClearBtn = true });
                builder.CloseComponent();
            };
        }

        /// <summary>
        /// Datepicker with period dropdown grid filter
        /// </summary>
        public static RenderFragment FilterDatePickerOp<T>(this GridState<T> state, ComponentBase receiver, Column<T> column)
        {
            var name = column.Id;
            var opName = name + "_Op";
            var opval = "=";

            return builder =>
            {
                builder.OpenComponent<ODatePicker>(0);
                builder.AddAttribute(1, "ValueChanged", EventCallback.Factory.Create<DateTime?>(receiver, v => state.FilterValChange(name, v)));
                builder.AddAttribute(2, "Value", state.GetFilterValue(name));
                builder.AddAttribute(3, "Opt", new DatePickerOpt { CssClass = "o-first" });
                builder.CloseComponent();

                BuildDateOpDropdownList(
                    builder,
                    "o-op",
                    EventCallback.Factory.Create<string>(receiver, v => state.FilterValChange(opName, v)),
                    (string)(state.GetFilterValue(opName) ?? opval));

                //BuildButton(builder,
                //    "awe-clrbtn",
                //    EventCallback.Factory.Create<MouseEventArgs>(receiver, (args) => state.ClearFilterValsForAndLoad(name)),
                //    "<span class=\"awe-icon awe-icon-x\"></span>");

                BuildClearAllButton(builder, EventCallback.Factory.Create<MouseEventArgs>(receiver, (args) => state.ClearFilterValsForAndLoad(name)));
            };
        }

        private static void BuildDateOpDropdownList<T>(RenderTreeBuilder builder, string cssClass, EventCallback<T> valueChanged, object value)
        {
            builder.OpenRegion(0);
            builder.OpenComponent<ODropdownList<string>>(1);
            builder.AddAttribute(2, "Opt", new DropdownOpt { CssClass = cssClass, Data = DateOpData() });
            builder.AddAttribute(3, "ValueChanged", valueChanged);
            builder.AddAttribute(4, "Value", value);
            builder.CloseComponent();
            builder.CloseRegion();
        }

        /// <summary>
        /// Button
        /// </summary>
        public static RenderFragment Button(ComponentBase receiver, string content, Action onClick)
        {
            return builder =>
            BuildButton(builder, null, EventCallback.Factory.Create<MouseEventArgs>(receiver, onClick), content);
        }

        private static void BuildButton(RenderTreeBuilder builder, string cssClass, EventCallback<MouseEventArgs> onClick, string content)
        {
            builder.OpenRegion(0);
            builder.OpenComponent<OButton>(1);
            builder.AddAttribute(2, "CssClass", cssClass);
            builder.AddAttribute(3, "OnClick", onClick);

            builder.AddAttribute(4, "ChildContent", (RenderFragment)((builder2) =>
            {
                builder2.AddContent(5, (MarkupString)content);
            }));

            builder.AddContent(6, content);
            builder.CloseComponent();
            builder.CloseRegion();
        }

        private static void BuildClearAllButton(RenderTreeBuilder builder, EventCallback<MouseEventArgs> onClick)
        {
            builder.OpenRegion(0);
            builder.OpenComponent<OClearAllButton>(1);            
            builder.AddAttribute(2, "OnClick", onClick);
            builder.CloseComponent();
            builder.CloseRegion();
        }

        private static IEnumerable<KeyContent> DateOpData()
        {
            return new[]{
            new KeyContent("=", OLangDict.Get(ODictKey.Equal)),
            new KeyContent("<", OLangDict.Get(ODictKey.LessThan)),
            new KeyContent("<=", OLangDict.Get(ODictKey.LessThanOrEqual)),
            new KeyContent(">", OLangDict.Get(ODictKey.GreaterThan)),
            new KeyContent(">=", OLangDict.Get(ODictKey.GreaterThanOrEqual)),
            };
        }

        private static Func<ColumnEditorContext<T>, RenderFragment> InlMulti<T>(IGridStateProp<T> gopt, InlDropdownPrm prm, Type compType)
        {
            return (cx) =>
            {
                return builder =>
                {
                    var receiver = gopt.State.Component;
                    var state = cx.EditItemState;
                    var column = cx.Column;
                    var input = state.Input;

                    var propName = prm.Name ?? column.Bind;
                    var propertyInfo = input.GetType().GetProperty(propName);

                    IEnumerable<object> getVal(object inp)
                    {
                        var val = gopt.State.GetBindValue(propName, inp).First();
                        if (val is null) return null;
                        return ((IEnumerable)val).Cast<object>();
                    }

                    var propValExpr = Expression.Property(Expression.Constant(input), propertyInfo);

                    var objPropValExpr = Expression.Convert(Expression.Property(Expression.Constant(input), propertyInfo), typeof(object));
                    var objPropLambda = Expression.Lambda<Func<object>>(objPropValExpr);

                    var propValParam = Expression.Parameter(typeof(object));

                    var castMethodInfo = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(new[] { propertyInfo.PropertyType.GetGenericArguments()[0] });

                    var cond = Expression.Condition(Expression.Equal(propValParam, Expression.Constant(null, propertyInfo.PropertyType)),
                        Expression.Constant(null, propertyInfo.PropertyType),
                        Expression.Call(castMethodInfo, Expression.Convert(propValParam, typeof(IEnumerable)))
                        );

                    var assignPropValExpr = Expression.Assign(propValExpr, cond);

                    var assignLambda = Expression.Lambda<Action<object>>(assignPropValExpr, propValParam).Compile();

                    builder.OpenComponent<OFieldInl>(0);
                    builder.AddAttribute(1, "For", RuntimeHelpers.TypeCheck(objPropLambda));

                    builder.AddAttribute(2, "ChildContent",
                        (RenderFragment)
                        (builder2 =>
                        {
                            CreateMulti(
                                builder2,
                                3,
                                4,
                                prm.Opt,
                                5,
                                getVal(input),
                                6,
                                EventCallback.Factory.Create(receiver, RuntimeHelpers.CreateInferredEventCallback<IEnumerable<object>>(receiver, assignLambda, getVal(input))),
                                7,
                                objPropLambda,
                                compType);
                        }));

                    builder.CloseComponent(); // close ofield
                };
            };
        }

        /// <summary>
        /// DropdownList grid inline editor
        /// </summary>
        public static Func<ColumnEditorContext<T>, RenderFragment> DropdownList<T>(IGridStateProp<T> gopt, InlDropdownPrm prm)
        {
            return (cx) =>
            {
                return builder =>
                {
                    var receiver = gopt.State.Component;
                    var state = cx.EditItemState;
                    var column = cx.Column;
                    var input = state.Input;

                    var propName = prm.Name ?? column.Bind;
                    object getVal(object inp) => gopt.State.GetBindValue(propName, inp).First();

                    var prop = input.GetType().GetProperty(propName);

                    var propValExpr = Expression.Property(Expression.Constant(input), prop);

                    var objPropValExpr = Expression.Convert(Expression.Property(Expression.Constant(input), prop), typeof(object));
                    var objPropLambda = Expression.Lambda<Func<object>>(objPropValExpr);

                    var propValParam = Expression.Parameter(typeof(object));
                    var assignPropValExpr = Expression.Assign(propValExpr, Expression.Convert(propValParam, prop.PropertyType));

                    var assignLambda = Expression.Lambda<Action<object>>(assignPropValExpr, propValParam).Compile();

                    builder.OpenComponent<OFieldInl>(0);
                    builder.AddAttribute(1, "For", RuntimeHelpers.TypeCheck(objPropLambda));

                    builder.AddAttribute(2, "ChildContent",
                        (RenderFragment)
                        (builder2 =>
                        {
                            CreateODropdownList(
                                builder2,
                                3,
                                4,
                                prm.Opt,
                                5,
                                getVal(input),
                                6,
                                EventCallback.Factory.Create(receiver, RuntimeHelpers.CreateInferredEventCallback(receiver, assignLambda, getVal(input))),
                                7,
                                objPropLambda);
                        }));

                    builder.CloseComponent(); // close ofield
                };
            };
        }

        private static Func<ColumnEditorContext<T>, RenderFragment> InlChk<T, TComp>(IGridStateProp<T> gopt) where TComp : IComponent
        {
            return (cx) =>
            {
                return builder =>
                {
                    var receiver = gopt.State.Component;
                    var state = cx.EditItemState;
                    var column = cx.Column;
                    var input = state.Input;

                    var propName = column.Bind;
                    bool getVal(object inp) => (bool)gopt.State.GetBindValue(propName, inp).First();
                    var prop = input.GetType().GetProperty(propName);
                    var inputPropValExpr = Expression.Property(Expression.Constant(input), prop);

                    var objPropValExpr = Expression.Convert(Expression.Property(Expression.Constant(input), prop), typeof(object));

                    var fieldForLambda = Expression.Lambda<Func<object>>(objPropValExpr);

                    var valueExprLambda = Expression.Lambda<Func<bool>>(inputPropValExpr);

                    var propValParam = Expression.Parameter(typeof(bool));
                    var assignPropValExpr = Expression.Assign(inputPropValExpr, propValParam);
                    var assignLambda = Expression.Lambda<Action<bool>>(assignPropValExpr, propValParam).Compile();

                    builder.OpenComponent<OFieldInl>(0);
                    builder.AddAttribute(1, "For", RuntimeHelpers.TypeCheck(fieldForLambda));
                    builder.AddAttribute(2, "ChildContent",
                        (RenderFragment)
                        (builder2 =>
                        {
                            builder2.OpenComponent<TComp>(3);
                            builder2.AddAttribute(4, "Value", RuntimeHelpers.TypeCheck(getVal(input)));
                            builder2.AddAttribute(5,
                                "ValueChanged",
                                RuntimeHelpers.TypeCheck(EventCallback.Factory.Create(
                                    receiver,
                                    RuntimeHelpers.CreateInferredEventCallback(receiver, assignLambda, getVal(input))
                                    )));

                            builder2.AddAttribute(6, "ValueExpression", RuntimeHelpers.TypeCheck(valueExprLambda));

                            builder2.CloseComponent();
                        }));

                    builder.CloseComponent();
                };
            };
        }

        private static void CreateODropdownList<TKey>(
            RenderTreeBuilder builder,
            int seq,
            int __seq0,
            DropdownOpt opt,
            int __seq1,
            TKey
            value,
            int __seq2,
            EventCallback<TKey> valueChanged,
            int __seq3,
            Expression<Func<object>> valueExpr)
        {
            builder.OpenComponent<ODropdownList<TKey>>(seq);
            builder.AddAttribute(__seq0, "Opt", opt);
            builder.AddAttribute(__seq1, "Value", value);
            builder.AddAttribute(__seq2, "ValueChanged", valueChanged);
            builder.AddAttribute(__seq3, "ValueExpression", valueExpr);
            builder.CloseComponent();
        }

        private static void CreateMulti<TKey>(
            RenderTreeBuilder builder,
            int seq,
            int __seq0,
            DropdownOpt opt,
            int __seq1,
            IEnumerable<TKey> value,
            int __seq2,
            EventCallback<IEnumerable<TKey>> valueChanged,
            int __seq3,
            Expression<Func<object>> valueExpr,
            Type compType)
        {
            builder.OpenComponent(seq, compType);
            builder.AddAttribute(__seq0, "Opt", opt);
            builder.AddAttribute(__seq1, "Value", value);
            builder.AddAttribute(__seq2, "ValueChanged", valueChanged);
            builder.AddAttribute(__seq3, "ValueExpression", valueExpr);
            builder.CloseComponent();
        }
    }
}