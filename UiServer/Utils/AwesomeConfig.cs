using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Omu.BlazorAwesome.Core;
using System;

namespace UiServer.Utils
{
    public static class AwesomeConfig
    {
        public static void Configure(IServiceProvider sprovider, bool sqlUsed = false)
        {
            var localizer = sprovider.GetService<IStringLocalizer<AweMui>>();

            OLangDict.GetTextFunc = key => localizer.GetString(key);

            AweSettings.StringFilterIgnoreCase = !sqlUsed;
        }
    }
}
