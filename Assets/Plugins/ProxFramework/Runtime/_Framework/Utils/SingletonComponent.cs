using System;
using UnityEngine;

namespace ProxFramework.Base
{
    public class SingletonComponent<T> : MonoBehaviour where T:MonoBehaviour
    {
        private static T _instance;
        
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var go = new GameObject("SingletonComponent");
                        DontDestroyOnLoad(go);
                        _instance = go.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }
    }
}