using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ProxFramework.Pool
{
    public class ObjectPool<T> where T : class
    {
        private readonly Queue<T> _pool = new();
        private readonly List<T> _activeList = new();
        private readonly Func<T> _createFunc;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;

        public ObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _onGet = onGet;
            _onRelease = onRelease;
        }

        public T Get(Action<T> initialize = null)
        {
            T obj = _pool.Count > 0 ? _pool.Dequeue() : _createFunc();
            _activeList.Add(obj);
            _onGet?.Invoke(obj);
            try
            {
                initialize?.Invoke(obj);
            }
            catch (Exception e)
            {
                Release(obj);
                PLogger.Error($"Get failed : {e}");
            }

            return obj;
        }

        public async UniTask GetAsync(Func<T, UniTask> asyncInitialize = null)
        {
            T obj = _pool.Count > 0 ? _pool.Dequeue() : _createFunc();
            _activeList.Add(obj);
            _onGet?.Invoke(obj);
            if (asyncInitialize == null)
            {
                return;
            }

            try
            {
                //TODO: UniTask.AttachExternalCancellation
                await asyncInitialize(obj);
            }
            catch (Exception e)
            {
                Release(obj);
                PLogger.Error($"GetAsync failed : {e}");
            }
        }

        public async UniTask PrewarmAsync(int count)
        {
            var list = new List<T>(count);
            for (var i = 0; i < count; i++)
            {
                var obj = _createFunc();
                _onRelease?.Invoke(obj);
                await UniTask.Yield();
                list.Add(obj);
            }

            foreach (var obj in list)
            {
                _pool.Enqueue(obj);
            }
        }

        public void Release(T obj)
        {
            if (obj == null) return;
            _activeList.Remove(obj);
            _onRelease?.Invoke(obj);
            _pool.Enqueue(obj);
        }

        public void ReleaseAll()
        {
            foreach (var obj in _activeList)
            {
                _onRelease?.Invoke(obj);
                _pool.Enqueue(obj);
            }

            _activeList.Clear();
        }

        public IEnumerable<T> GetAllActiveObjects() => _activeList.ToArray();

        public void ForEachActive(Action<T> action)
        {
            foreach (var obj in _activeList.ToArray())
            {
                action(obj);
            }
        }
    }
}