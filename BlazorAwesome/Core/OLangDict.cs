using System;
using System.Collections.Generic;

namespace Omu.BlazorAwesome.Core
{
    /// <summary>
    /// Language dictionary
    /// </summary>
    public static class OLangDict
    {
        /// <summary>
        /// 
        /// </summary>
        public static Func<string, string> GetTextFunc { get; set; }

        /// <summary>
        /// Get text by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {            
            if (GetTextFunc is not null)
            {
                var res = GetTextFunc(key);
                if (res is not null && res != key) return res;
            }            

            if (EmbDict.ContainsKey(key)) return EmbDict[key];

            return key;
        }

        internal static void Get(Func<object, bool> equals)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dictionary storage
        /// </summary>
        public static readonly Dictionary<string, string> EmbDict = new()
        {
            { ODictKey.GroupBar, "Drag a column header and drop it here to group by that column" },
            { ODictKey.PageSize, "page size" },
            { ODictKey.PageInfo, "of {0} items" },
            { ODictKey.PleaseSelect, "please select" },
            { ODictKey.SearchToLoadMoreResults, "search to load more results" },
            { ODictKey.LessThan, "Less than" },
            { ODictKey.LessThanOrEqual, "Less than or equal" },
            { ODictKey.GreaterThan, "Greather than" },
            { ODictKey.GreaterThanOrEqual, "Greather than or equal" },
        };
    }
}