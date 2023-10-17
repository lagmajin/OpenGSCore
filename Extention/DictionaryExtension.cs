using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public static  class DictionaryExtension
    {
        public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> dic, TK key, TV defaultValue = default) => dic.TryGetValue(key, out var result) ? result : defaultValue;

        //public static TValue GetValueOrDefalut<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TKey, TValue> func);
    }
}
