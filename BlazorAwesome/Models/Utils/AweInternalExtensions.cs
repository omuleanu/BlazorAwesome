using System.Collections.Generic;
using System.Linq;

namespace Omu.BlazorAwesome.Models.Utils
{
    internal static class AweInternalExtensions
    {
        public static bool IsCollectionEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static T CloneShallow<T>(this T target, T source)
        {
            var sourceProps = source.GetType().GetProperties();

            var targetType = typeof(T);

            foreach (var sp in sourceProps)
            {
                if (!sp.CanRead || sp.GetGetMethod() == null) continue;

                var tp = targetType.GetProperty(sp.Name);

                if (tp != null && tp.CanWrite && sp.PropertyType == tp.PropertyType && tp.GetSetMethod() != null)
                {
                    tp.SetValue(target, sp.GetValue(source, null), null);
                }
            }

            return target;
        }
    }
}