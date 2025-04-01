using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGSCore
{
    public static class JObjectExtensions
    {
        public static bool ContainsAllKeys(this JObject jObject, params string[] keys)
        {
            return keys.All(key =>
            {
                string jsonPath = key.Replace("/", "."); // `/` を `.` に統一
                return jObject.SelectToken(jsonPath) != null;
            });
        }
    }

}