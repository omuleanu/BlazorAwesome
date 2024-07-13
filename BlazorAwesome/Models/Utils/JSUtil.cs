using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Omu.BlazorAwesome.Models.Utils
{    
    internal static class JSUtil
    {
        public static async Task<IJSObjectReference> InvokeAsync(IJSRuntime JS, string identifier, object obj)
        {
            try
            {
                return await JS.InvokeAsync<IJSObjectReference>(identifier, obj);
            }
            catch (ObjectDisposedException)
            {
            }

            return null;
        }

        public static async Task InvokeVoidAsync(IJSRuntime JS, string identifier, object obj)
        {
            try
            {
                await JS.InvokeVoidAsync(identifier, obj);
            }
            catch (ObjectDisposedException)
            {
            }

            return;
        }
    }
}