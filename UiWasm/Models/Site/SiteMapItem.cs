using Omu.BlazorAwesome.Models;

namespace UiWasm.Models.Site
{
    public class SiteMapItem : KeyContent
    {
        public SiteMapItem(string href, string text) : base(href, text)
        {
        }

        public string Keywords { get; set; }

        public string Title { get; set; }
    }
}