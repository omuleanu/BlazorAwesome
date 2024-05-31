using Microsoft.AspNetCore.Localization;
using Omu.BlazorAwesome.Models;
using System.Collections.Generic;
using System.Globalization;

namespace UiServer.Utils
{
    public static class Globals
    {
        public static List<ThemeItem> Themes { get; } = new()
        {
            new ThemeItem("mat", new[] {"#fff", "#f3f6f9", "#3481e5"}),
            new ThemeItem("start", new[] {"rgb(228 230 233)", "#e4e6e9", "rgb(24, 119, 242)"}),
            new ThemeItem("bts", new[] {"#fff", "#6c757d", "#007bff"}),
            new ThemeItem("mui", new[] {"#fff", "#fff", "#26a69a"}),
            new ThemeItem("gui3", new[] {"#F4F9F9", "#F8F8F8", "#4D90FE"}),
            new ThemeItem("zen", new[] {"#c7c8ca", "#fff", "#333"}),
            new ThemeItem("wui", new[] {"#fafafa", "#f8f8f8", "#f8f8f8"}),
            new ThemeItem("gui", new[] {"#F0F2F2", "#FFD814", "#007185"}),
            new ThemeItem("val", new[] {"#fdf5f7", "#e77f94", "#b383c7"}),            
            new ThemeItem("met", new[] {"#f7f7f7", "#36A9E1", "#75a532"}),
            new ThemeItem("gtx", new[] {"rgb(17, 17, 17)", "#2d2d2d", "#76b900"}),
            new ThemeItem("black-cab", new[] {"#586779", "#445869", "#3f88b1"}),
        };

        public const string ThemeCookieName = "awetheme";

        public static string DefaultTheme => Themes[0].Key.ToString();

        public static string PickedTheme { get; set; }

        public static IList<CultureInfo> SupportedCultures { get; } = new List<CultureInfo>
        {
            new CultureInfo("en"),
            new CultureInfo("fr"),
            new CultureInfo("it"),
            new CultureInfo("es"),
            new CultureInfo("de"),
            new CultureInfo("pt"),
            new CultureInfo("ro"),
            new CultureInfo("ru"),
        };

        public static RequestCulture DefaultCulture { get; } = new RequestCulture("en");

        public static Dictionary<string, string> LangNames = new() {
            { "en", "English" },
            { "fr", "French" },
            { "it", "Italian" },
            { "es", "Spanish" },
            { "de", "German" },
            { "pt", "Portuguese" },
            { "ro", "Romanian" },
            { "ru", "Russian" },
            };
    }

    public class ThemeItem : KeyContent
    {
        public ThemeItem(string key, string[] colors)
        {
            Key = key;
            Content = key;
            Colors = colors;
        }

        public string[] Colors { get; set; }
    }
}