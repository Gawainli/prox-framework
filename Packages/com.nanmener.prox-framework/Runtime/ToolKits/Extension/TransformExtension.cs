using UnityEngine;

namespace ProxFramework.Utils
{
    public static class TransformExtension
    {
        public static void DestroyAllChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}