using System;
using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;
using Object = UnityEngine.Object;

namespace ProxFramework.UI
{
    public abstract class UIWindow
    {
        public const int WindowHideLayer = 2; // Ignore Raycast
        public const int WindowShowLayer = 5; // UI
        public const int WindowDepthStep = 10;

        private AssetHandle _assetHandle;
        private string _location;
        private bool _isCreate = false;
        private GameObject _panel;
        private Canvas _canvas;
        private Canvas[] _childCanvas;
        private GraphicRaycaster _rayCaster;
        private GraphicRaycaster[] _childRayCaster;
        private Action<UIWindow> _prepareCallback;
        public Transform Transform => GameObject.transform;
        public RectTransform RectTransform => GameObject.GetComponent<RectTransform>();
        public GameObject GameObject => _panel;
        public string WindowName { get; private set; }
        public int WindowLayer { get; private set; }
        public bool Prepared { get; private set; }
        public bool FullScreen { get; private set; }

        public object UserData
        {
            get
            {
                if (UserDatas is { Length: > 0 })
                {
                    return UserDatas[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public object[] UserDatas { get; internal set; }

        public int Depth
        {
            get => _canvas != null ? _canvas.sortingOrder : 0;
            set
            {
                if (_canvas != null)
                {
                    if (_canvas.sortingOrder == value)
                    {
                        return;
                    }

                    _canvas.sortingOrder = value;

                    // Set child canvas sorting order
                    int depth = value;
                    foreach (var child in _childCanvas)
                    {
                        if (child != null && child != _canvas)
                        {
                            depth += WindowDepthStep;
                            child.sortingOrder = depth;
                        }
                    }
                }

                if (_isCreate)
                {
                    OnSortDepth(value);
                }
            }
        }

        public bool Visible
        {
            get => _canvas != null && _canvas.gameObject.layer == WindowShowLayer;
            set
            {
                if (_canvas != null)
                {
                    int setLayer = value ? WindowShowLayer : WindowHideLayer;
                    if (_canvas.gameObject.layer == setLayer)
                    {
                        return;
                    }

                    _canvas.gameObject.layer = setLayer;

                    // Set child canvas layer
                    foreach (var child in _childCanvas)
                    {
                        if (child != null && child != _canvas)
                        {
                            child.gameObject.layer = setLayer;
                        }
                    }

                    Interactable = value;
                    if (_isCreate)
                    {
                        OnSetVisible(value);
                    }
                }
            }
        }

        public bool Interactable
        {
            get => _rayCaster && _rayCaster.enabled;
            set
            {
                if (_rayCaster && _rayCaster.enabled != value)
                {
                    _rayCaster.enabled = value;
                    foreach (var rc in _childRayCaster)
                    {
                        rc.enabled = value;
                    }
                }
            }
        }

        public void Init(string windowName, int windowLayer, bool fullScreen)
        {
            WindowName = windowName;
            WindowLayer = windowLayer;
            FullScreen = fullScreen;
        }

        /// <summary>
        /// 窗口创建
        /// </summary>
        public abstract void OnCreate();

        /// <summary>
        /// 窗口刷新
        /// </summary>
        public abstract void OnRefresh();

        /// <summary>
        /// 窗口更新
        /// </summary>
        public abstract void OnUpdate(float deltaTime);

        /// <summary>
        /// 窗口销毁
        /// </summary>
        public abstract void OnDestroy();

        /// <summary>
        /// 当触发窗口的层级排序
        /// </summary>
        protected virtual void OnSortDepth(int depth)
        {
        }

        /// <summary>
        /// 当因为全屏遮挡触发窗口的显隐
        /// </summary>
        protected virtual void OnSetVisible(bool visible)
        {
        }

        internal async UniTask InternalLoadAsync(string location)
        {
            if (_assetHandle != null)
            {
                return;
            }

            _assetHandle = AssetModule.GetAssetHandle<AssetHandle>(location, true);
            await _assetHandle;
            InternalLoadCompleted();
        }

        internal void InternalLoadSync(string location)
        {
            if (_assetHandle != null)
            {
                return;
            }

            _assetHandle = AssetModule.GetAssetHandle<AssetHandle>(location, false);
            InternalLoadCompleted();
        }

        private void InternalLoadCompleted()
        {
            if (_assetHandle.AssetObject == null)
            {
                PLogger.Error("UIWindow Load Error: panelPrefab is null.");
                return;
            }

            _panel = _assetHandle.InstantiateSync(UIModule.UIRoot.transform);
            _panel.transform.localPosition = Vector3.zero;

            _canvas = GameObject.GetComponent<Canvas>();
            if (_canvas == null)
            {
                PLogger.Error("UIWindow Load Error: Canvas is null. name: " + _panel.name);
                return;
            }

            _canvas.overrideSorting = true;
            _canvas.sortingOrder = 0;
            _canvas.sortingLayerName = "UI";

            _rayCaster = GameObject.GetComponent<GraphicRaycaster>();
            _childCanvas = GameObject.GetComponentsInChildren<Canvas>();
            _childRayCaster = GameObject.GetComponentsInChildren<GraphicRaycaster>();
        }

        protected Transform Q(string path)
        {
            return GameObject.transform.Find(path);
        }

        protected T Q<T>(string path) where T : Component
        {
            var transform = GameObject.transform.Find(path);
            if (transform == null)
            {
                PLogger.Error($"{WindowName} Q Error: {path} is null");
            }

            return transform.GetComponent<T>();
        }


        internal void InternalCreate()
        {
            if (_isCreate)
            {
                return;
            }

            _isCreate = true;
            OnCreate();
        }

        internal void InternalRefresh(params System.Object[] userDatas)
        {
            if (!_isCreate)
            {
                return;
            }

            UserDatas = userDatas;
            OnRefresh();
        }

        internal void InternalUpdate(float deltaTime)
        {
            if (!_isCreate || !Visible || !Prepared)
            {
                return;
            }

            OnUpdate(deltaTime);
        }

        internal void InternalDestroy()
        {
            _isCreate = false;
            _prepareCallback = null;

            AssetModule.ReleaseAsset(_assetHandle.AssetObject);
            _assetHandle = null;

            if (_panel == null) return;
            OnDestroy();
            Object.Destroy(_panel);
            _panel = null;
        }
    }
}