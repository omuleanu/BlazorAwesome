using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Omu.BlazorAwesome.Models;

namespace Omu.BlazorAwesome.Components.Internal
{
    /// <summary>
    /// Awesome Editor base
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class OEditorBase<TValue> : ComponentBase
    {
        /// <summary>        
        /// </summary>
        protected bool IsDisabled
        {
            get
            {
                var opt = getOpt();
                if (opt != null && opt.Disabled.HasValue) return opt.Disabled.Value;

                if (OContext != null && OContext.Opt != null)
                {
                    return OContext.Opt.Disabled;
                }

                return false;
            }
        }

        private Func<Task> afterChangeFunc
        {
            get
            {
                var opt = getOpt();
                if (opt != null && opt.AfterChangeFunc != null) return opt.AfterChangeFunc;

                if (OContext != null && OContext.Opt != null) return OContext.Opt.AfterChangeFunc;

                return null;
            }
        }

        /// <summary>
        /// Call after change func
        /// </summary>
        protected async Task afterChange()
        {
            if (afterChangeFunc is not null)
            {
                await afterChangeFunc();
            }
        }

        /// <summary>
        /// Get Opt config model
        /// </summary>        
        protected virtual IEditorOpt getOpt()
        {
            return null;
        }

        /// <summary>
        /// Cascading Awesome Context
        /// </summary>
        [CascadingParameter] protected OContext OContext { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the bound value.
        ///</summary>
        [Parameter] public Expression<Func<TValue>> ValueExpression { get; set; }

        /// <summary>
        /// Gets or sets the display name for this field.
        /// <para>This value is used when generating error messages when the input value fails to parse correctly.</para>
        /// </summary>
        [Parameter] public string DisplayName { get; set; }

        /// <summary>        
        /// </summary>
        protected EditContext EditContext { get; set; } = default!;

        [CascadingParameter] EditContext CascadedEditContext { get; set; } = default!;

        /// <summary>
        /// Gets the <see cref="FieldIdentifier"/> for the bound value.
        /// </summary>
        protected internal FieldIdentifier FieldIdentifier { get; set; }

        /// <inheritdoc />
        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (EditContext == null)
            {
                // This is the first run
                // Could put this logic in OnInit, but its nice to avoid forcing people who override OnInit to call base.OnInit()

                if (CascadedEditContext != null)
                {
                    EditContext = CascadedEditContext;
                }

                if (ValueExpression != null)
                {
                    FieldIdentifier = FieldIdentifier.Create(ValueExpression);
                }
            }
            else if (CascadedEditContext != EditContext)
            {
                // Not the first run

                // We don't support changing EditContext because it's messy to be clearing up state and event
                // handlers for the previous one, and there's no strong use case. If a strong use case
                // emerges, we can consider changing this.
                throw new InvalidOperationException($"{GetType()} does not support changing the " +
                                                    $"{nameof(EditContext)} dynamically.");
            }

            // For derived components, retain the usual lifecycle with OnInit/OnParametersSet/etc.
            return base.SetParametersAsync(ParameterView.Empty);
        }

        /// <summary>
        /// Get css class for the main div (field)
        /// </summary>
        protected virtual string fieldClass
        {
            get
            {
                var opt = getOpt();
                var cc = opt?.CssClass;
                var res = IsDisabled ? "awe-disabled " : string.Empty;

                if (cc != null)
                {
                    res += " " + cc;
                }

                return addFieldValidClass(res);
            }
        }

        private string addFieldValidClass(string res)
        {
            if (EditContext != null && FieldIdentifier.FieldName != null)
            {
                res += " " + EditContext.FieldCssClass(FieldIdentifier);
            }

            return res;
        }

        /// <summary>
        /// NotifyFieldChanged, if possible
        /// </summary>
        protected void FieldChanged()
        {
            if (FieldIdentifier.FieldName != null)
            {
                EditContext?.NotifyFieldChanged(FieldIdentifier);
            }
        }
    }
}