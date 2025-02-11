using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using ProxFramework.Runtime.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ProxFramework.UI
{
    public static class UIModule
    {
        private static bool _initialized;
        private static readonly List<UIWindow> _windowStack = new List<UIWindow>(100);
        private static RectTransform _rootRectTransform;
        public static GameObject UIRoot { get; private set; }
        public static RectTransform RootRectTransform => _rootRectTransform;

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
                window.UserDatas = userDatas;
                Pop(window);
                Push(window);
                OnWindowPrepared(window);
                return (T)window;
            }
            else
            {
                var window = CreateWindowInstance(typeof(T));
                window.UserDatas = userDatas;
                Push(window);
                await window.InternalLoadAsync(assetPath);
                OnWindowPrepared(window);
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
                window.UserDatas = userDatas;
                Pop(window);
                Push(window);
                OnWindowPrepared(window);
                return (T)window;
            }
            else
            {
                var window = CreateWindowInstance(typeof(T));
                window.UserDatas = userDatas;
                Push(window);
                window.InternalLoadSync(assetPath);
                return (T)window;
            }
        }

        private static void OnWindowPrepared(UIWindow window, params object[] userDatas)
        {
            SortWindowDepth(window.WindowLayer);
            window.InternalCreate();
            window.InternalRefresh();
            SetWindowVisible();
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
                window.InternalDestroy();
                Pop(window);
                SortWindowDepth(window.WindowLayer);
                SetWindowVisible();
            }
        }

        public static void CloseAll()
        {
            for (int i = 0; i < _windowStack.Count; i++)
            {
                _windowStack[i].InternalDestroy();
            }

            _windowStack.Clear();
        }

        private static GameObject GetUIRootOrCreate(UISettings uiSettings)
        {
            if (UIRoot != null)
            {
                return UIRoot;
            }
            
            PLogger.Info("Create UIRootCanvas");
            var root = new GameObject("UIRootCanvas");
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = uiSettings.pixelPerfect;
            var scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.matchWidthOrHeight = uiSettings.matchWidthOrHeight ? 0 : 1;
            scaler.referenceResolution = new Vector2(uiSettings.referenceResolutionX, uiSettings.referenceResolutionY);
            var raycaster = root.AddComponent<GraphicRaycaster>();
            raycaster.ignoreReversedGraphics = uiSettings.ignoreReversedGraphics;
            return root;
        }

        private static UIWindow CreateWindowInstance(Type type)
        {
            var window = Activator.CreateInstance(type) as UIWindow;
            if (window == null)
            {
                PLogger.Error($"Create window failed. Type:{type.FullName}");
                return null;
            }

            var attribute = Attribute.GetCustomAttribute(type, typeof(UIWindowAttribute)) as UIWindowAttribute;
            if (attribute == null)
            {
                PLogger.Error($"Window attribute not found. Type:{type.FullName}");
                return null;
            }

            window.Init(type.FullName, attribute.windowLayer, attribute.fullScreen);
            return window;
        }

        private static void Push(UIWindow window)
        {
            if (window == null)
            {
                PLogger.Error("Push window is null.");
                return;
            }

            if (ContainsWindow(window.WindowName))
            {
                PLogger.Error($"Window already exists. WindowName:{window.WindowName}");
                return;
            }

            var index = -1;
            for (int i = 0; i < _windowStack.Count; i++)
            {
                if (window.WindowLayer < _windowStack[i].WindowLayer)
                {
                    index = i + 1;
                }
            }

            if (index == -1)
            {
                for (int i = 0; i < _windowStack.Count; i++)
                {
                    if (window.WindowLayer > _windowStack[i].WindowLayer)
                    {
                        index = i + 1;
                    }
                }
            }

            if (index == -1)
            {
                index = 0;
            }

            _windowStack.Insert(index, window);
        }

        private static void Pop(UIWindow window)
        {
            _windowStack.Remove(window);
        }

        public static bool ContainsWindow<T>()
        {
            return ContainsWindow(typeof(T).FullName);
        }

        public static bool ContainsWindow(string name)
        {
            foreach (var ui in _windowStack)
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
            foreach (var ui in _windowStack)
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
            for (int i = _windowStack.Count - 1; i >= 0; i--)
            {
                UIWindow window = _windowStack[i];
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
            for (int i = 0; i < _windowStack.Count; i++)
            {
                if (_windowStack[i].WindowLayer == layer)
                {
                    _windowStack[i].Depth = depth;
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
        
        public static Vector3 UIPosToWorldPos(Vector2 uiPos)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_rootRectTransform, uiPos, Camera.main, out var worldPos);
            return worldPos;
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
                    PLogger.Info(
                        $"ExecuteEvents.Execute Success {result.gameObject.name} {function.Method.Name} EventType:{typeof(T).Name}");
                    if (passOneFirst && eventData.used)
                    {
                        break;
                    }
                }
                else
                {
                    PLogger.Info(
                        $"ExecuteEvents.Execute Fail {result.gameObject.name} EventType:{typeof(T).Name}");
                    //如果当前object能响应click事件，但是没有响应，那么不再向下传递
                    if (passOneFirst && result.gameObject.GetComponent<IPointerClickHandler>() != null)
                    {
                        break;
                    }
                }
            }
        }


        public static void Initialize(GameObject uiRoot = null)
        {
            if (_initialized)
            {
                PLogger.Warning("UIModule already initialized.");
                return;
            }

            UIRoot = uiRoot??GetUIRootOrCreate(SettingsUtil.GlobalSettings.uiSettings);
            _rootRectTransform = UIRoot.GetComponent<RectTransform>();
        }

        public static void Tick(float deltaTime)
        {
            if (!_initialized) return;
            foreach (var ui in _windowStack)
            {
                ui?.InternalUpdate(deltaTime);
            }
        }

        public static void Shutdown()
        {
            if (!_initialized)
            {
                return;
            }

            CloseAll();
            _initialized = false;
            if (UIRoot != null)
            {
                Object.Destroy(UIRoot);
            }
        }
    }
}