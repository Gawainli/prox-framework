using System;
using System.Collections.Generic;
using GameName.Base.PlaySys;
using ProxFramework;

namespace GameName.Base
{
    public class GameSingletonSystem : AbsGameSys
    {
        private class Wrapper
        {
            public int Priority { private set; get; }
            public ISingleton Singleton { private set; get; }

            public Wrapper(ISingleton module, int priority)
            {
                Singleton = module;
                Priority = priority;
            }
        }

        private bool _isInit;
        private int _maxPriority;
        private readonly List<Wrapper> _wrappers = new List<Wrapper>(100);
        private bool _isDirty;

        public override void Init()
        {
            if (_isInit)
            {
                return;
            }

            _maxPriority = -999;
            _isInit = true;
        }

        public override void Update(float deltaTime, float unscaledDeltaTime)
        {
            if (!_isInit)
            {
                return;
            }

            if (_isDirty)
            {
                _wrappers.Sort((x, y) => x.Priority.CompareTo(y.Priority));
                _isDirty = false;
            }

            foreach (var wrapper in _wrappers)
            {
                wrapper.Singleton.OnUpdate();
            }
        }

        public override void Shutdown()
        {
            if (!_isInit)
            {
                return;
            }
            
            DestroyAll();
            _isInit = false;
        }

        public T CreateSingleton<T>(int priority = 0) where T : class, ISingleton
        {
            return CreateSingleton<T>(null, priority);
        }

        public T CreateSingleton<T>(object createParam, int priority = 0) where T : class, ISingleton
        {
            if (priority < 0)
            {
                PLogger.Error($"Singleton priority must be greater than 0 : {typeof(T)}");
                return null;
            }
            
            if (Contains<T>())
            {
                PLogger.Warning($"Singleton already exists : {typeof(T)}. return old instance");
                return GetSingleton<T>();
            }

            // 如果没有设置优先级
            if (priority == 0)
            {
                // 如果没有设置最大优先级
                if (_maxPriority == -999)
                {
                    _maxPriority = 0;
                }
                else
                {
                    priority = _maxPriority + 100;
                    _maxPriority = priority;
                }
            }
            else
            {
                // 如果设置了优先级
                if (priority > _maxPriority)
                {
                    _maxPriority = priority;
                }
            }

            var singleton = Activator.CreateInstance<T>();
            var wrapper = new Wrapper(singleton, priority);
            wrapper.Singleton.OnCreate(createParam);
            _wrappers.Add(wrapper);
            _isDirty = true;
            return singleton;
        }
        
        public bool DestroySingleton<T>() where T : class, ISingleton
        {
            var type = typeof(T);
            for (int i = 0; i < _wrappers.Count; i++)
            {
                if (_wrappers[i].Singleton.GetType() == type)
                {
                    _wrappers[i].Singleton.OnDestroy();
                    _wrappers.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        
        private void DestroyAll()
        {
            foreach (var wrapper in _wrappers)
            {
                wrapper.Singleton.OnDestroy();
            }

            _wrappers.Clear();
        }

        public bool Contains<T>() where T : class, ISingleton
        {
            System.Type type = typeof(T);
            for (int i = 0; i < _wrappers.Count; i++)
            {
                if (_wrappers[i].Singleton.GetType() == type)
                    return true;
            }

            return false;
        }

        public T GetSingleton<T>() where T : class, ISingleton
        {
            var type = typeof(T);
            foreach (var wrapper in _wrappers)
            {
                if (wrapper.Singleton.GetType() == type)
                    return wrapper.Singleton as T;
            }

            PLogger.Error($"Not found manager : {type}");
            return null;
        }
    }
}