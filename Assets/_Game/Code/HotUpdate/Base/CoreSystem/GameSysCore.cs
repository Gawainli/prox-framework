using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameName.DataTable;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameName.Base.PlaySys
{
    public static class GameSysCore
    {
        private static GameObject _driver;
        private static MonoBehaviour _behaviour;
        private static bool _initialized;
        private static Dictionary<string, AbsGameSys> _mapNameToSystem = new();

        public static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            _driver = new GameObject($"[{nameof(GameSysCore)}]");
            _behaviour = _driver.AddComponent<GameSysBehaviour>();
            Object.DontDestroyOnLoad(_driver);
        }

        public static bool RegisterSystem(AbsGameSys system)
        {
            if (_mapNameToSystem.TryAdd(system.GetType().Name, system))
            {
                system.Init();
                return true;
            }

            return false;
        }

        public static bool RegisterSystem<T>() where T : AbsGameSys, new()
        {
            var system = new T();
            if (_mapNameToSystem.TryAdd(system.GetType().Name, system))
            {
                system.Init();
                return true;
            }

            return false;
        }

        public static bool ContainsSystem<T>() where T : AbsGameSys
        {
            return _mapNameToSystem.ContainsKey(typeof(T).Name);
        }

        public static T GetSystem<T>() where T : AbsGameSys
        {
            var sys = _mapNameToSystem.GetValueOrDefault(typeof(T).Name);
            return sys as T;
        }

        public static void FixedUpdate(float fixedTime)
        {
            foreach (var system in _mapNameToSystem.Values)
            {
                system.FixedUpdate(fixedTime);
            }
        }

        public static void Update(float deltaTime, float unscaledDeltaTime)
        {
            foreach (var system in _mapNameToSystem.Values)
            {
                system.Update(deltaTime, unscaledDeltaTime);
            }
        }

        public static void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            foreach (var system in _mapNameToSystem.Values)
            {
                system.LateUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        public static void OnApplicationPause(bool pause)
        {
            foreach (var system in _mapNameToSystem.Values)
            {
                system.OnApplicationPause(pause);
            }
        }

        public static void OnApplicationFocus(bool focus)
        {
            foreach (var system in _mapNameToSystem.Values)
            {
                system.OnApplicationFocus(focus);
            }
        }

        public static void OnApplicationQuit()
        {
            foreach (var system in _mapNameToSystem.Values)
            {
                system.OnApplicationQuit();
            }
        }

        public static void OnDrawGizmos()
        {
            foreach (var system in _mapNameToSystem.Values)
            {
                system.OnDrawGizmos();
            }
        }

        public static void OnDrawGizmosSelected()
        {
            foreach (var system in _mapNameToSystem.Values)
            {
                system.OnDrawGizmosSelected();
            }
        }

        public static void Shutdown()
        {
            if (!_initialized)
            {
                return;
            }

            foreach (var system in _mapNameToSystem.Values)
            {
                system.Shutdown();
            }

            _mapNameToSystem.Clear();
            StopAllCoroutines();
            Object.Destroy(_driver);
            _initialized = false;
        }

        #region Coroutine

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return _behaviour.StartCoroutine(routine);
        }

        public static Coroutine StartCoroutine(string methodName, object value = null)
        {
            return _behaviour.StartCoroutine(methodName, value);
        }

        public static void StopCoroutine(IEnumerator routine)
        {
            _behaviour.StopCoroutine(routine);
        }

        public static void StopCoroutine(Coroutine routine)
        {
            _behaviour.StopCoroutine(routine);
        }

        public static void StopCoroutine(string methodName)
        {
            _behaviour.StopCoroutine(methodName);
        }

        public static void StopAllCoroutines()
        {
            _behaviour.StopAllCoroutines();
        }

        #endregion
    }
}