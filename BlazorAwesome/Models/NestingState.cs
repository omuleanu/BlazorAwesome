using Omu.BlazorAwesome.Models.Utils;
using System.Collections.Generic;

namespace Omu.BlazorAwesome.Models
{
    /// <summary>
    /// Grid nesting state
    /// </summary>
    public class NestingState<T>
    {
        private GridState<T> gs;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gs"></param>
        public NestingState(GridState<T> gs)
        {
            this.gs = gs;
        }

        private string getnkey(T item)
        {
            return item is null ? string.Empty : gs.GetKey(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, Nest> OpenedNests { get; } = new();

        /// <summary>
        /// get nests current state css class 
        /// </summary>        
        public string NestClass(object key)
        {
            if (getNest(key.ToString()) is not null)
            {
                return Consts.OpenNestClass;
            }

            return string.Empty;
        }

        /// <summary>
        /// Close opened nest
        /// </summary>
        /// <param name="key"></param>
        public void Close(object key)
        {   
            var k = key.ToString();
            if (OpenedNests.ContainsKey(k))
            {
                OpenedNests.Remove(k);
            }
        }

        private Nest getNest(string key)
        {
            if (OpenedNests.ContainsKey(key))
            {
                return OpenedNests[key];
            }

            return null;
        }

        /// <summary>
        /// Open or close given grid nest
        /// </summary>
        /// <param name="nest"></param>
        public void ToggleOpen(Nest nest)
        {
            var key = nest.Key.ToString();
            
            if (OpenedNests.ContainsKey(key))
            {
                var exNest = OpenedNests[key];
                OpenedNests.Remove(key);

                if (exNest.Key.Equals(nest.Key) && exNest.Name == nest.Name)
                {
                    return;
                }

                // open different type of nest
            }

            OpenedNests.Add(key, nest);
        }
    }
}