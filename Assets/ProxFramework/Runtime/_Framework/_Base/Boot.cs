using System;
using ProxFramework.Asset;
using ProxFramework.Event;
using ProxFramework.Module;
using ProxFramework.Network;
using ProxFramework.Pool;
using ProxFramework.StateMachine;
using ProxFramework.UI;
using UnityEngine;
using YooAsset;

namespace ProxFramework.Base
{
    public class Boot : MonoBehaviour, IModule
    {
        public EPlayMode resPlayMode = EPlayMode.EditorSimulateMode;
        public string packageName = "DefaultPackage";
        public string rawPackageName = "DefaultRawPackage";
        public string hostServerIP = "http://localhost:8080";
        public string appVersion = "v1.0";
        public GameObject uiRoot;
        public int defaultFrameRate = 60;

        private StateMachine.StateMachine _stateMachine;

        public void Initialize(object userData = null)
        {
            SetSettings();

            ModuleCore.CreateModule<StateMachineModule>();
            ModuleCore.CreateModule<EventModule>();

            var resCfg = new AssetModuleCfg
            {
                assetPkgName = packageName,
                rawPkgName = rawPackageName,
                ePlayMode = resPlayMode,
                hostServerIP = hostServerIP,
                appVersion = appVersion
            };
            ModuleCore.CreateModule<AssetModule>(resCfg);
            ModuleCore.CreateModule<PoolModule>(new PoolModuleCfg()
            {
                pkg = AssetModule.assetPkg,
                poolingRoot = gameObject
            });
            ModuleCore.CreateModule<NetworkModule>();

            if (uiRoot != null)
            {
                ModuleCore.CreateModule<UIModule>(uiRoot);
            }

            // _stateMachine = StateMachineModule.Create<Boot>(this);
            // _stateMachine.AddState<StatePatchPrepare>();
            // _stateMachine.AddState<StateInitPackage>();
            // _stateMachine.AddState<StateUpdateVersion>();
            // _stateMachine.AddState<StateUpdateManifest>();
            // _stateMachine.AddState<StateCreateDownloader>();
            // _stateMachine.AddState<StateDownloadFile>();
            // _stateMachine.AddState<StateClearCache>();
            // _stateMachine.AddState<StatePatchDone>();
            // _stateMachine.AddState<StateStartGame>();
            // _stateMachine.AddState<StateLoadAssembly>();

            Initialized = true;
        }

        private void SetSettings()
        {
            SettingUtils.playMode = resPlayMode;
            Application.targetFrameRate = defaultFrameRate;
            Logger.Info($"Boot SetSettings playMode:{resPlayMode} frameRate:{defaultFrameRate}");
        }

        public void Tick(float deltaTime, float unscaledDeltaTime)
        {
            ModuleCore.TickAllModules(deltaTime, unscaledDeltaTime);
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public int Priority { get; set; } = 9999;
        public bool Initialized { get; set; }

        #region Mono

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            // _stateMachine.Start<StatePatchPrepare>();
        }

        private void Update()
        {
            Tick(Time.deltaTime, Time.unscaledDeltaTime);
        }

        #endregion
    }
}