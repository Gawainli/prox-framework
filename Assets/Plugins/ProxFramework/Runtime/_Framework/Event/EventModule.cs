using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;

namespace ProxFramework.Event
{
    public static class EventModule
    {
        private sealed class EventQueue
        {
            public readonly Queue<IEvent> events = new();
            public readonly Dictionary<Type, List<Func<IEvent, UniTask>>> eventHandlers = new();
        }

        private static EventQueue _eventQueue;
        private static bool _initialized;

        public static void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            var wrapper = new Func<T, UniTask>(e =>
            {
                handler(e);
                return UniTask.CompletedTask;
            });

            InternalSubscribe(wrapper);
        }

        public static void Subscribe<T>(Func<T, UniTask> handler) where T : IEvent
        {
            InternalSubscribe(handler);
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            var type = typeof(T);
            if (!_eventQueue.eventHandlers.TryGetValue(type, out var handlers))
            {
                return;
            }

            var wrapper = new Func<IEvent, UniTask>(e =>
            {
                handler((T)e);
                return UniTask.CompletedTask;
            });

            handlers.Remove(wrapper);
        }

        public static void UnsubscribeAll<T>() where T : IEvent
        {
            var type = typeof(T);
            if (!_eventQueue.eventHandlers.TryGetValue(type, out var handlers))
            {
                return;
            }

            handlers.Clear();
        }

        public static void Unsubscribe<T>(Func<T, UniTask> handler) where T : IEvent
        {
            var type = typeof(T);
            if (!_eventQueue.eventHandlers.TryGetValue(type, out var handlers))
            {
                return;
            }

            handlers.RemoveAll(h =>
                h.Target == handler.Target && h.Method.Name == handler.Method.Name
            );
        }

        public static void Publish<T>(T message) where T : IEvent
        {
            _eventQueue.events.Enqueue(message);
        }

        public static void Tick(float delta)
        {
            if (!_initialized)
            {
                return;
            }

            ProcessEvents().Forget();
        }

        public static void Initialize()
        {
            _eventQueue = new EventQueue();
            _initialized = true;
        }

        public static void Shutdown()
        {
            _initialized = false;
            _eventQueue = null;
        }

        private static void InternalSubscribe<T>(Func<T, UniTask> handler) where T : IEvent
        {
            var type = typeof(T);
            if (!_eventQueue.eventHandlers.TryGetValue(type, out var handlers))
            {
                handlers = new List<Func<IEvent, UniTask>>();
                _eventQueue.eventHandlers[type] = handlers;
            }

            handlers.Add(e => handler((T)e));
        }

        private static async UniTask ProcessEvents()
        {
            while (_eventQueue.events.Count > 0)
            {
                var e = _eventQueue.events.Dequeue();
                var type = e.GetType();
                if (!_eventQueue.eventHandlers.TryGetValue(type, out var handlers))
                {
                    continue;
                }

                var tasks = new List<UniTask>();
                foreach (var handler in handlers)
                {
                    try
                    {
                        tasks.Add(handler(e));
                    }
                    catch (Exception exception)
                    {
                        PLogger.Error($"Error processing event {type.Name}: {exception}");
                    }
                }

                await UniTask.WhenAll(tasks);
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Prox/RuntimeTools/Print Queue Status")]
        private static void PrintQueueStatus()
        {
            if (!_initialized)
            {
                PLogger.Info("EventModule not initialized");
                return;
            }
            
            var status = new StringBuilder()
                .AppendLine($"Pending events: {_eventQueue.events.Count}")
                .AppendLine($"Registered handlers:");

            foreach (var kvp in _eventQueue.eventHandlers)
            {
                status.AppendLine($"- {kvp.Key.Name}: {kvp.Value.Count} handlers");
            }

            PLogger.Info(status.ToString());
        }
#endif
    }
}