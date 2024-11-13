using System.Collections.Generic;
using UnityEngine.Pool;

namespace ProxFramework.Pool
{
    public static class ListPoolExtensions
    {
        public static void Release2Pool<T>(this List<T> self)
        {
            ListPool<T>.Release(self);
        }
    }
}