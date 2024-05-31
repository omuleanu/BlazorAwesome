using Omu.BlazorAwesome.Core;

namespace Omu.BlazorAwesome.Models
{
    internal static class RenderUtil
    {
        public static string GetColumnHeaderCss<T>(ColumnState<T> cs)
        {
            var res = cs.Column.HeaderCssClass ?? string.Empty;

            if (cs.Column.Bind != null)
            {
                if (cs.Column.Sortable.Value)
                {
                    res += " awe-sortable";
                }

                if (cs.Column.Groupable.Value)
                {
                    res += " awe-groupable";
                }

                if (cs.Column.Reorderable.Value)
                {
                    res += " awe-rer";
                }
            }

            if (cs.Sort == Sort.Asc)
            {
                res += " awe-asc";
            }
            else if (cs.Sort == Sort.Desc)
            {
                res += " awe-desc";
            }

            return res;
        }

        public static string ColStyle<T>(Column<T> col)
        {
            var result = string.Empty;

            if (col.Width > 0)
            {
                result = "width:" + col.Width + "px";
            }

            return result;
        }
    }
}