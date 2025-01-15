using System;
using System.Collections.Generic;
using ProxFramework.Base;

using ProxFramework.Module;
using UnityEngine;

namespace ProxFramework.Event
{
    public class EventModule : IModule
    {
        private class PostWrapper
        {
            public int postFrame;
            public int eventId;
            public IEventMessage message;

            public void Recycle()
            {
                postFrame = 0;
                eventId = 0;
                message = null;
            }
        }

        private static readonly Dictionary<int, List<Action<IEventMessage>>> Listeners =
            new Dictionary<int, List<Action<IEventMessage>>>();

        private static readonly List<PostWrapper> PostList = new List<PostWrapper>();

        public static void AddListener(int eventId, Action<IEventMessage> listener)
        {
            if (Listeners.TryGetValue(eventId, out var list))
            {
                list.Add(listener);
            }
            else
            {
                list = new List<Action<IEventMessage>>();
                list.Add(listener);
                Listeners.Add(eventId, list);
            }
        }

        public static void AddListener<TEvent>(Action<IEventMessage> listener) where TEvent : IEventMessage
        {
            var type = typeof(TEvent);
            AddListener(type.GetHashCode(), listener);
        }

        public static void RemoveListener(int eventId, Action<IEventMessage> listener)
        {
            if (Listeners.TryGetValue(eventId, out var list))
            {
                if (listener != null)
                {
                    list.Remove(listener);
                }
                else
                {
                    list.Clear();
                }
            }
        }

        public static void RemoveListener<TEvent>(Action<IEventMessage> listener = null) where TEvent : IEventMessage
        {
            var type = typeof(TEvent);
            RemoveListener(type.GetHashCode(), listener);
        }

        public static void ClearALlListener()
        {
            Listeners.Clear();
            PostList.Clear();
        }

        public static void SendEvent(int eventId, IEventMessage msg)
        {
            if (Listeners.TryGetValue(eventId, out var list))
            {
                // foreach (var listener in list)
                // {
                //     if (listener != null)
                //     {
                //         listener(msg);
                //     }
                // }
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var listener = list[i];
                    if (listener != null)
                    {
                        listener(msg);
                    }
                }
            }
        }

        public static void SendEvent(IEventMessage msg)
        {
            var eventId = msg.GetType().GetHashCode();
            SendEvent(eventId, msg);
        }
        
        public static void SendEvent<TEvent>(TEvent msg) where TEvent : IEventMessage
        {
            var eventId = typeof(TEvent).GetHashCode();
            SendEvent(eventId, msg);
        }

        public static void PostEvent(int eventId, IEventMessage msg)
        {
            var wrapper = new PostWrapper
            {
                postFrame = Time.frameCount,
                eventId = eventId,
                message = msg
            };
            PostList.Add(wrapper);
        }

        public static void PostEvent(IEventMessage msg)
        {
            var eventId = msg.GetType().GetHashCode();
            PostEvent(eventId, msg);
        }
        
        public static void PostEvent<TEvent>(TEvent msg) where TEvent : IEventMessage
        {
            var eventId = typeof(TEvent).GetHashCode();
            PostEvent(eventId, msg);
        }

        public void Initialize(object userData = null)
        {
            PLogger.Info("EventModule Initialize");
            Initialized = true;
        }

        public void Tick(float deltaTime, float unscaledDeltaTime)
        {
            for (int i = PostList.Count - 1; i >= 0; i--)
            {
                var wrapper = PostList[i];
                if (Time.frameCount - wrapper.postFrame > 1)
                {
                    SendEvent(wrapper.eventId, wrapper.message);
                    wrapper.Recycle();
                    PostList.RemoveAt(i);
                }
            }
        }

        public void Shutdown()
        {
            ClearALlListener();
        }

        public int Priority { get; set; }
        public bool Initialized { get; set; }
    }
}