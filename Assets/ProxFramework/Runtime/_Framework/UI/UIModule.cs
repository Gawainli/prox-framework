using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using ProxFramework.Module;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProxFramework.UI
{
    public class UIModuleCfg
    {
        public bool pixelPerfect = false;
        public bool matchWidthOrHeight = false;
        public float referenceResolutionX = 1920;
        public float referenceResolutionY = 1080;
        public bool ignoreReversedGraphics = false;
    }

    public class UIModule : IModule
    {
        private static readonly List<UIWindow> WindowStack = new List<UIWindow>(100);
        private static RectTransform _rootRectTransform;

        public static GameObject UIRoot { get; private set; }

        public static async UniTask<T> OpenWindowAsync<T>(string assetPath, params object[] userDatas)
            where T : UIWindow
        {
            return await InternalOpenWindowAsync<T>(assetPath, userDatas);
        }

        private static async UniTask<T> InternalOpenWindowAsync<T>(string assetPath, params object[] userDatas)
            where T : UIWindow
        {
            var windowName = typeof(T).FullName;
            if (ContainsWindow(windowName))
            {
                var window = GetWindow(windowName);
                PopWindow(window);
                PushWindow(window);
                SortWindowDepth(window.WindowLayer);
                window.Create();
                window.Refresh(userDatas);
                return (T)window;
            }
            else
            {
                var window = CreateWindowInstance(typeof(T));
                PushWindow(window);
                await window.LoadAsync(assetPath, userDatas);
                SortWindowDepth(window.WindowLayer);
                window.Create();
                window.Refresh(userDatas);
                return (T)window;
            }
        }


        public static T OpenWindowSync<T>(string assetPath, int layer = 0, bool fullscreen = false,
            params System.Object[] userDatas)
            where T : UIWindow
        {
            var windowName = typeof(T).FullName;
            if (ContainsWindow(windowName))
            {
                var window = GetWindow(windowName);
                PopWindow(window);
                PushWindow(window);
                SortWindowDepth(window.WindowLayer);
                window.Create();
                window.Refresh(userDatas);
                return (T)window;
            }
            else
            {
                var window = CreateWindowInstance(typeof(T));
                PushWindow(window);
                window.LoadSync(assetPath, userDatas);
                SortWindowDepth(window.WindowLayer);
                window.Create();
                window.Refresh(userDatas);
                return (T)window;
            }
        }

        public static void CloseWindow(Type type)
        {
            CloseWindow(type.FullName);
        }

        public static void CloseWindow<T>() where T : UIWindow
        {
            CloseWindow(typeof(T).FullName);
        }

        public static void CloseWindow(string windowName)
        {
            if (ContainsWindow(windowName))
            {
                var window = GetWindow(windowName);
                window.Destroy();
                PopWindow(window);
                //TODO:关闭窗口时对window重新排序
                SortWindowDepth(window.WindowLayer);
                SetWindowVisible();
            }
        }

        public static void CloseAll()
        {
            for (int i = 0; i < WindowStack.Count; i++)
            {
                WindowStack[i].Destroy();
            }

            WindowStack.Clear();
        }

        private GameObject NewUIRoot(UIModuleCfg cfg)
        {
            var root = new GameObject("UIRoot");
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = cfg.pixelPerfect;
            var scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.matchWidthOrHeight = cfg.matchWidthOrHeight ? 0 : 1;
            scaler.referenceResolution = new Vector2(cfg.referenceResolutionX, cfg.referenceResolutionY);
            var raycaster = root.AddComponent<GraphicRaycaster>();
            raycaster.ignoreReversedGraphics = cfg.ignoreReversedGraphics;
            return root;
        }

        private static UIWindow CreateWindowInstance(Type type)
        {
            var window = Activator.CreateInstance(type) as UIWindow;
            if (window == null)
            {
                Logger.Error($"Create window failed. Type:{type.FullName}");
                return null;
            }

            var attribute = Attribute.GetCustomAttribute(type, typeof(UIWindowAttribute)) as UIWindowAttribute ??
                            new UIWindowAttribute(0, false);
            window.Init(type.FullName, attribute.windowLayer, attribute.fullScreen);
            return window;
        }

        private static void PushWindow(UIWindow window)
        {
            if (window == null)
            {
                return;
            }

            var index = -1;
            for (int i = 0; i < WindowStack.Count; i++)
            {
                if (window.WindowLayer < WindowStack[i].WindowLayer)
                {
                    index = i + 1;
                }
            }

            if (index == -1)
            {
                for (int i = 0; i < WindowStack.Count; i++)
                {
                    if (window.WindowLayer > WindowStack[i].WindowLayer)
                    {
                        index = i + 1;
                    }
                }
            }

            if (index == -1)
            {
                index = 0;
            }

            WindowStack.Insert(index, window);
        }

        private static void PopWindow(UIWindow window)
        {
            WindowStack.Remove(window);
        }

        public static bool ContainsWindow<T>()
        {
            return ContainsWindow(typeof(T).FullName);
        }

        public static bool ContainsWindow(string name)
        {
            foreach (var ui in WindowStack)
            {
                if (ui.WindowName == name)
                {
                    return true;
                }
            }

            return false;
        }

        public static UIWindow GetWindow(string name)
        {
            foreach (var ui in WindowStack)
            {
                if (ui.WindowName == name)
                {
                    return ui;
                }
            }

            return null;
        }

        public static T GetWindow<T>() where T : UIWindow
        {
            return GetWindow(typeof(T).FullName) as T;
        }

        private static void SetWindowVisible()
        {
            bool isHideNext = false;
            for (int i = WindowStack.Count - 1; i >= 0; i--)
            {
                UIWindow window = WindowStack[i];
                if (isHideNext == false)
                {
                    window.Visible = true;
                    if (window.Prepared && window.FullScreen)
                        isHideNext = true;
                }
                else
                {
                    window.Visible = false;
                }
            }
        }

        private static void SortWindowDepth(int layer)
        {
            int depth = layer;
            for (int i = 0; i < WindowStack.Count; i++)
            {
                if (WindowStack[i].WindowLayer == layer)
                {
                    WindowStack[i].Depth = depth;
                    depth += 100; //注意：每次递增100深度
                }
            }
        }

        public static Vector2 WorldPosToUIPos(Vector3 worldPos)
        {
            var screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rootRectTransform, screenPos, null, out var uiPos);
            return uiPos;
        }

        //从当前 game object 开始向下传递事件。默认只传递到下一级execute event成功的 object
        //unity的click事件比较特殊，必须execute down和up事件才会触发click事件。所以如果传递到的object需要响应click，但传递的event execute失败,也视为响应成功不再向下传递
        public static void PassEvent<T>(GameObject callObj, PointerEventData eventData,
            ExecuteEvents.EventFunction<T> function, bool passOneFirst = true)
            where T : IEventSystemHandler
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count == 0)
            {
                return;
            }

            var idx = 0;
            for (var i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject == callObj)
                {
                    //只传给自己下层的handler
                    idx = i + 1;
                    break;
                }
            }

            for (; idx < results.Count; idx++)
            {
                var result = results[idx];
                if (ExecuteEvents.Execute(result.gameObject, eventData, function))
                {
                    Logger.Info(
                        $"ExecuteEvents.Execute Success {result.gameObject.name} {function.Method.Name} EventType:{typeof(T).Name}");
                    if (passOneFirst && eventData.used)
                    {
                        break;
                    }
                }
                else
                {
                    Logger.Info(
                        $"ExecuteEvents.Execute Fail {result.gameObject.name} EventType:{typeof(T).Name}");
                    //如果当前object能响应click事件，但是没有响应，那么不再向下传递
                    if (passOneFirst && result.gameObject.GetComponent<IPointerClickHandler>() != null)
                    {
                        break;
                    }
                }
            }
        }

        #region IModule

        public void Initialize(object userData = null)
        {
            Logger.Info("UIModule Initialize");
            if (userData is UnityEngine.GameObject uiRoot)
            {
                UIRoot = uiRoot;
            }
            else if (userData is UIModuleCfg cfg)
            {
                UIRoot = NewUIRoot(cfg);
            }
            else if (userData == null)
            {
                UIRoot = NewUIRoot(new UIModuleCfg());
            }

            _rootRectTransform = UIRoot.GetComponent<RectTransform>();
            Initialized = true;
        }

        public void Tick(float deltaTime, float unscaledDeltaTime)
        {
            if (Initialized)
            {
                foreach (var ui in WindowStack)
                {
                    ui?.Update(deltaTime);
                }
            }
        }

        public void Shutdown()
        {
        }

        public int Priority { get; set; }
        public bool Initialized { get; set; }

        #endregion
    }
}