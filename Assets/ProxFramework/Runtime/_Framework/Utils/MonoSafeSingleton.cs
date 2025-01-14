using System;

using UnityEngine;

namespace ProxFramework.Base
{
    using UnityEngine;

    public class MonoSafeSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (_applicationIsQuitting)
                    {
                        Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                         "' already destroyed on application quit." +
                                         " Won't create again - returning null.");
                        return null; 
                    }
                    
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = FindObjectOfType<T>();

                            if (FindObjectsOfType<T>().Length > 1)
                            {
                                ProxFramework.Logger.Error("[Singleton] Something went really wrong " +
                                                                  " - there should never be more than 1 singleton!" +
                                                                  " Reopening the scene might fix it.");
                                return _instance;
                            }

                            if (_instance == null)
                            {
                                var singleton = new GameObject();
                                _instance = singleton.AddComponent<T>();
                                singleton.name = typeof(T).ToString() + " (Singleton)";


                                ProxFramework.Logger.Info("[Singleton] An instance of " + typeof(T) +
                                                                 " is needed in the scene, so '" + singleton +
                                                                 "' was created with DontDestroyOnLoad.");
                            }
                            else
                            {
                                ProxFramework.Logger.Info("[Singleton] Using instance already created: " +
                                                                 _instance.gameObject.name);
                            }

                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                }

                return _instance;
            }
        }

        private static bool _applicationIsQuitting = false;
        private void OnDestroy()
        {
            _applicationIsQuitting = true;
        }
    }
}