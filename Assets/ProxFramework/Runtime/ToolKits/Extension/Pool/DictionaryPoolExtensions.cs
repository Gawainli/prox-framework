using System.Collections.Generic;
using UnityEngine.Pool;

namespace ProxFramework.Pool
{
    public static class DictionaryPoolExtensions
    {
        public static void Release2Pool<TKey, TValue>(this Dictionary<TKey, TValue> self)
        {
            DictionaryPool<TKey, TValue>.Release(self);
        }
    }
}