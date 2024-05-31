using Omu.BlazorAwesome.Models;
using System;

namespace UiServer.Utils
{
    public static class OExtensions
    {
        public static DropdownOpt ImgItemRender(this DropdownOpt opt)
        {
            opt.CaptionFunc = RenderComp.ImgCaptionFunc;
            opt.ItemFunc = RenderComp.ImgItemFunc;
            return opt;
        }
    }
}
