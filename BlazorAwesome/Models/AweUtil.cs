using Omu.BlazorAwesome.Models.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Utils
    /// </summary>
    public static class AweUtil
    {
        /// <summary>
        /// init drag drop reorder, return js object reference
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="dragOpt"></param>
        /// <returns></returns>
        public static async Task<IJSObjectReference> InitDragReorderRet(IJSRuntime JS, DragOpt dragOpt)
        {
            var dto = dragOpt.ToDto();
            if (dragOpt.ObjRef == null)
            {
                // ref disposed; (moved to different page)
                return null;
            }

            return await JSUtil.InvokeAsync(JS, "awe.dragBz", dto);
        }

        /// <summary>
        /// init drag drop reorder
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="dragOpt"></param>
        /// <returns></returns>
        public static async Task InitDragReorder(IJSRuntime JS, DragOpt dragOpt)
        {
            var dto = dragOpt.ToDto();
            if (dragOpt.ObjRef == null && dragOpt.DropMethod != null)
            {
                return;
                // ref disposed; (moved to different page)                
            }

            await JSUtil.InvokeVoidAsync(JS, "awe.dragBz", dto);
            
            return;
        }

        /// <summary>
        /// init grid reorder rows
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static async Task InitGridReorderRows(IJSRuntime JS, GridReorderRowsOpt opt)
        {
            await AweUtil.InitDragReorder(JS, new()
            {
                Cont = opt.Cont,
                FromSelector = ".awe-itc:first",
                Selector = ".awe-row",

                ToCont = opt.Cont,
                ToSelector = ".awe-itc:first",

                PlaceholderClass = "awe-hl",
                DragWrapJSFunc = "awe.rowDragWrap",
                ObjRef = opt.ObjRef,
                DropMethod = opt.DropMethod,
                HandleSelector = opt.HandleSelector
            });
        }

        /// <summary>
        /// flash grid row after render
        /// </summary>
        public static void FlashRow<T>(IGridStateProp<T> gopt, object key)
        {
            gopt.State.Component.AddPostRenderAction(async () =>
            {
                await gopt.State.Component.JS.InvokeVoidAsync(
                    CompUtil.AweJs("flashRow"),
                    new { gdiv = gopt.State.Component.Div, key });
            });
        }

        /// <summary>
        /// json serialize
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        /// <summary>
        /// json deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string str)
        {
            return JsonSerializer.Deserialize<T>(str);
        }

        /// <summary>
        /// get page count
        /// </summary>
        /// <param name="count"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int GetPageCount(long count, int pageSize)
        {
            return (int)Math.Ceiling((float)count / pageSize);
        }

        /// <summary>
        /// Get a page from a list of items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="take">number of items to take, by default it is pageSize</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IQueryable<T> GetPage<T>(IQueryable<T> items, int page, int pageSize, int take = 0)
        {
            if (take == 0) take = pageSize;
            return items.Skip((page - 1) * pageSize).Take(take);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="models"></param>
        /// <param name="prop"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IEnumerable<KeyContent> ToKeyContent<TModel>(
            IEnumerable<TModel> models,
            Func<TModel, object> prop,
            Func<TModel, string> content)
        {
            return models.Select(o => new KeyContent { Key = prop(o), Content = content(o) });
        }
    }
}