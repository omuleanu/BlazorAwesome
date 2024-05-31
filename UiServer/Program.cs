using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Omu.BlazorAwesome.Core;
using UiServer.Data;
using UiServer.Resources;
using UiServer.Utils;

var builder = WebApplication.CreateBuilder(args);

#region Services
var services = builder.Services;

services.AddRazorPages();
services.AddServerSideBlazor();

services.AddTransient<DbSeeder>();
services.AddSingleton<CachedItems>();

#region Database

var sqlUsed = false;

/// to use Sql Server instead of InMemory Database, 
/// uncomment the lines after "sql server db" comment and comment out the line after "InMemory db"
/// modify the connection string in appsettings.json if necessary
/// the Db will be recreated on start by the DbSeeder

// sql server db
//var cs = builder.Configuration.GetConnectionString("awecs");
//services.AddDbContextFactory<MyContext>(o => o.UseSqlServer(cs), ServiceLifetime.Singleton);
//sqlUsed = true;

// InMemory db
services.AddDbContextFactory<MyContext>(opt => opt.UseInMemoryDatabase("awedemo"), ServiceLifetime.Singleton);
#endregion

#region Localization services
services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
services.AddSingleton<SharedViewLocalizer>();
#endregion

#endregion

var app = builder.Build();

app.Services.GetService<DbSeeder>().Seed(true); // true parameter recreates database

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

#region Localization

// set localization and filter settings for awesome
AwesomeConfig.Configure(app.Services, sqlUsed);

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = Globals.DefaultCulture,
    // Formatting numbers, dates, etc.
    SupportedCultures = Globals.SupportedCultures,
    // UI strings that we have localized.
    SupportedUICultures = Globals.SupportedCultures
});
#endregion

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();