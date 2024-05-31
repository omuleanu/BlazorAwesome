using Omu.BlazorAwesome.Models;

namespace UiWasm.Models
{
    public class ImgKeyContent : KeyContent
    {
        public ImgKeyContent(){}

        public ImgKeyContent(object key, string content, string url)
        {
            Key = key;
            Content = content;
            Url = url;
        }

        public string Url { get; set; }
    }
}