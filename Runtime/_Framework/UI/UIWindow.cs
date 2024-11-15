using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
using ProxFramework.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace ProxFramework.UI
{
    public abstract class UIWindow
    {

        private GameObject _uiPanel;
        private Canvas _canvas;
        private Canvas[] _childCanvas;

        private bool _isCreate = false;

        private GraphicRaycaster _rayCaster;
        private GraphicRaycaster[] _childRayCaster;

        public Transform Transform => _uiPanel.transform;
        public RectTransform RectTransform => _uiPanel.GetComponent<RectTransform>();
        public GameObject GameObject => _uiPanel;
        public string WindowName { get; private set; }
        public int WindowLayer { get; private set; }
        public bool Prepared { get; private set; }
        public bool FullScreen { get; private set; }

        private object[] _userDatas;
        public object UserData
        {
            get
            {
                if (_userDatas is { Length: > 0 })
                {
                    return _userDatas[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public object[] UserDatas => _userDatas;

        protected UIWindow()
        {
        }

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
                    int depth = value;
                    foreach (var child in _childCanvas)
                    {
                        if (child != null && child != _canvas)
                        {
                            depth++;
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
            get => _uiPanel != null && _uiPanel.activeSelf;
            set
            {
                if (_uiPanel != null && _uiPanel.activeSelf != value)
                {
                    _uiPanel.SetActive(value);

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

        public async UniTask LoadAsync(string assetPath, System.Object[] userDatas)
        {
            _userDatas = userDatas;
            var uiPrefab = await AssetModule.LoadAssetAsync<GameObject>(assetPath);
            if (uiPrefab == null)
            {
                LogModule.Error("UIWindow Load Error: uiPrefab is null. path: " + assetPath);
                return;
            }

            InstantiatePanel(uiPrefab);
        }

        public void LoadSync(string assetPath, System.Object[] userDatas)
        {
            _userDatas = userDatas;
            var uiPrefab = AssetModule.LoadAssetSync<GameObject>(assetPath);
            if (uiPrefab == null)
            {
                LogModule.Error("UIWindow Load Error: panelPrefab is null. path: " + assetPath);
                return;
            }

            InstantiatePanel(uiPrefab);
        }

        private void InstantiatePanel(GameObject panelPrefab)
        {
            _uiPanel = Object.Instantiate(panelPrefab, UIModule.UIRoot.transform);
            _uiPanel.transform.localPosition = Vector3.zero;
            ;
            _canvas = _uiPanel.GetComponent<Canvas>();
            if (_canvas == null)
            {
                LogModule.Error("UIWindow Load Error: Canvas is null. name: " + panelPrefab.name);
            }

            _canvas.overrideSorting = true;
            _canvas.sortingOrder = 0;
            _canvas.sortingLayerName = "UI";

            // var rectTransform = _uiPanel.GetComponent<RectTransform>();
            // var rootRectTransform = UIModule.UIRoot.GetComponent<RectTransform>();
            // rectTransform.sizeDelta = rootRectTransform.sizeDelta;
            // rectTransform.anchoredPosition = rootRectTransform.anchoredPosition;
            // rectTransform.pivot = new Vector2(0.5f, 0.5f);
            // rectTransform.transform.localScale = Vector3.one;


            _rayCaster = _uiPanel.GetComponent<GraphicRaycaster>();
            _childCanvas = _uiPanel.GetComponentsInChildren<Canvas>();
            _childRayCaster = _uiPanel.GetComponentsInChildren<GraphicRaycaster>();
            Prepared = true;
        }

        protected Transform Q(string path)
        {
            return _uiPanel.transform.Find(path);
        }

        protected T Q<T>(string path) where T : Component
        {
            var transform = _uiPanel.transform.Find(path);
            if (transform == null)
            {
                LogModule.Error($"{WindowName} Q Error: {path} is null");
            }

            return transform.GetComponent<T>();
        }


        internal void Create()
        {
            if (_isCreate)
            {
                return;
            }

            _isCreate = true;
            OnCreate();
        }

        internal void Refresh(params System.Object[] userDatas)
        {
            if (!_isCreate)
            {
                return;
            }

            _userDatas = userDatas;
            OnRefresh();
        }

        internal void Update(float deltaTime)
        {
            if (!_isCreate)
            {
                return;
            }

            OnUpdate(deltaTime);
        }

        internal void Destroy()
        {
            if (!_isCreate)
            {
                return;
            }

            if (_uiPanel != null)
            {
                OnDestroy();
                Object.Destroy(_uiPanel);
                _uiPanel = null;
            }
        }
    }
}