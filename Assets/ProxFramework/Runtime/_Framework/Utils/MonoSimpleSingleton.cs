
using UnityEngine;

namespace ProxFramework.Utils
{
    public class MonoSimpleSingleton<T> : MonoBehaviour where T : MonoSimpleSingleton<T> 
    {
        public static T instance;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = (T)this;
                gameObject.name = $"{gameObject.name} (SimpleSingleton)";
            }
            else
            {
                Logger.Warning($"More than one single:{nameof(T)}. Destroy this game object:{gameObject.name}");
                Destroy(gameObject);
            }
        }
    }
}