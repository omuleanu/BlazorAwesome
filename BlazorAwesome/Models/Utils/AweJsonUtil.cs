using System;
using System.Globalization;
using System.Text.Json;

namespace Omu.BlazorAwesome.Models.Utils
{
    /// <summary>
    /// Utility class for parsing to and from AweJsonObj
    /// </summary>
    public static class AweJsonUtil
    {
        private static string getTypeName(Type type)
        {
            return type.FullName;
        }

        /// <summary>
        /// parse object to AweJsonObj
        /// </summary>
        public static AweJsonObj ToAweJsonObj(object val)
        {
            var res = new AweJsonObj
            {
                Type = getTypeName(val.GetType())
            };

            if (res.Type == getTypeName(typeof(DateTime)))
            {
                res.Value = ((DateTime)val).ToString("o");
            }
            else
            {
                res.Value = AweUtil.Serialize(val);
            }

            return res;
        }

        /// <summary>
        /// Parse AweJsonObj to object
        /// </summary>        
        public static object FromAweJsonObj(AweJsonObj jo)
        {
            object res;
            if (jo.Type == getTypeName(typeof(DateTime)))
            {
                res = DateTime.ParseExact(jo.Value, "o", CultureInfo.InvariantCulture);
            }
            else
            {
                var type = Type.GetType(jo.Type);
                var val = JsonSerializer.Deserialize(jo.Value, type);
                res = Convert.ChangeType(val, type);
            }

            return res;
        }
    }
}