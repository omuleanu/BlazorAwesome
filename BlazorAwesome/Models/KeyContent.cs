namespace Omu.BlazorAwesome.Models
{    
    /// <summary>
    /// 
    /// </summary>
    public class KeyContent
    {
        /// <summary>
        /// 
        /// </summary>
        public KeyContent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        public KeyContent(object key, object content)
        {
            Key = key;
            Content = content.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public object Key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; }
    }
}