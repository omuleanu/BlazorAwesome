using UiWasm;
using UiWasm.Data;
using UiWasm.Resources;
using UiWasm.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Omu.BlazorAwesome.Core;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;

#region Localization services
services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
services.AddSingleton<SharedViewLocalizer>();
#endregion

services.AddSingleton<CachedItems>();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var app = builder.Build();

// set localization and filter settings for awesome
AwesomeConfig.Configure(app.Services);

var js = app.Services.GetRequiredService<IJSRuntime>();
var lang = await js.InvokeAsync<string>("site.getLSVal", "lang"); //  get lang from local storage
var theme = await js.InvokeAsync<string>("site.getLSVal", Globals.ThemeCookieName);

if (theme != null)
{
    Globals.PickedTheme = theme;
}

if (lang != null)
{
    var culture = new CultureInfo(lang);
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}

await app.RunAsync();