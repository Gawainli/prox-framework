using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ProxFramework.Base;
using ProxFramework.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace ProxFramework.Asset
{
    internal class ResLogger : YooAsset.ILogger
    {
        public void Log(string message)
        {
            PLogger.Info(message);
        }

        public void Warning(string message)
        {
            PLogger.Warning(message);
        }

        public void Error(string message)
        {
            PLogger.Error(message);
        }

        public void Exception(Exception exception)
        {
            PLogger.Exception(exception.ToString());
        }
    }

    public static partial class AssetModule
    {
        private static bool _initialized = false;
        private static int _downloadingMaxNum;
        private static int _failedTryAgain;
        private static Dictionary<string, ResourcePackage> _mapNameToResourcePackage = new();

        private static EPlayMode _playMode;

        public static EPlayMode PlayMode
        {
            get
            {
#if UNITY_EDITOR
                _playMode = SettingsUtil.EditorDevSettings.playMode;
#else
                playMode = SettingsUtil.GlobalSettings.assetSettings.playMode;
#endif
                return _playMode;
            }
        }

        public static string DefaultRawPkgName => SettingsUtil.GlobalSettings.assetSettings.defaultRawPackageName;
        public static TaskCtsModule.CtsInfo ctsInfo;
        public static CancellationToken CtsToken => ctsInfo.cts.Token;
        public static string DefaultPkgName => SettingsUtil.GlobalSettings.assetSettings.defaultPackageName;

        public static void Initialize()
        {
            if (!YooAssets.Initialized)
            {
                YooAssets.Initialize(new ResLogger());
            }

            ctsInfo = TaskCtsModule.GetCts();
            _downloadingMaxNum = SettingsUtil.GlobalSettings.assetSettings.maxDownloadingNum;
            _failedTryAgain = SettingsUtil.GlobalSettings.assetSettings.failedTryAgain;

            var defaultPackage = YooAssets.TryGetPackage(DefaultPkgName);
            if (defaultPackage == null)
            {
                defaultPackage = YooAssets.CreatePackage(DefaultPkgName);
            }

            YooAssets.SetDefaultPackage(defaultPackage);
            _initialized = true;
        }

        public static async UniTask<EOperationStatus> InitPackage(string packageName)
        {
            var pkgPlayMode = PlayMode;
#if UNITY_EDITOR
            pkgPlayMode = SettingsUtil.EditorDevSettings.GetPackageDevPlayMode(packageName);
#endif
            var package = YooAssets.TryGetPackage(packageName) ?? YooAssets.CreatePackage(packageName);
            _mapNameToResourcePackage.TryAdd(packageName, package);

            InitializationOperation initializationOperation = null;
            if (pkgPlayMode == EPlayMode.EditorSimulateMode)
            {
                var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                var packageRoot = buildResult.PackageRootDirectory;
                var createParameters = new EditorSimulateModeParameters();
                createParameters.EditorFileSystemParameters =
                    FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                initializationOperation = package.InitializeAsync(createParameters);
            }
            else if (pkgPlayMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                createParameters.BuildinFileSystemParameters =
                    FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                initializationOperation = package.InitializeAsync(createParameters);
            }
            else if (pkgPlayMode == EPlayMode.HostPlayMode)
            {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                var createParameters = new HostPlayModeParameters();
                createParameters.BuildinFileSystemParameters =
                    FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                createParameters.CacheFileSystemParameters =
                    FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                initializationOperation = package.InitializeAsync(createParameters);
            }
            else if (pkgPlayMode == EPlayMode.WebPlayMode)
            {
                var createParameters = new WebPlayModeParameters();
#if UNITY_WEBGL && WEIXINMINIGAME && !UNITY_EDITOR
			    string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                createParameters.WebServerFileSystemParameters =
 WechatFileSystemCreater.CreateWechatFileSystemParameters(remoteServices);
#else
                createParameters.WebServerFileSystemParameters =
                    FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
#endif
                initializationOperation = package.InitializeAsync(createParameters);
            }

            if (initializationOperation == null)
            {
                return EOperationStatus.Failed;
            }

            await initializationOperation.ToUniTask(cancellationToken: ctsInfo.cts.Token);
            return initializationOperation.Status;
        }

        private static string GetHostServerURL()
        {
            var hostUrl = SettingsUtil.GlobalSettings.assetSettings.assetCdn;
            var appVersion = Application.version;

#if UNITY_EDITOR
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                return $"{hostUrl}/android/{appVersion}";
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                return $"{hostUrl}/ios/{appVersion}";
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
                return $"{hostUrl}/webgl/{appVersion}";
            else
                return $"{hostUrl}/pc/{appVersion}";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostUrl}/android/{appVersion}";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostUrl}/ios/{appVersion}";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostUrl}/webgl/{appVersion}";
        else
            return $"{hostUrl}/pc/{appVersion}";
#endif
        }

        public static ResourcePackage[] GetAllPackages()
        {
            return _mapNameToResourcePackage.Values.ToArray();
        }

        public static ResourcePackage GetPackage(string packageName)
        {
            if (_mapNameToResourcePackage.TryGetValue(packageName, out var package))
            {
                return package;
            }

            PLogger.Error($"Package {packageName} not found");
            return null;
        }

        public static bool TryGetPackage(string packageName, out ResourcePackage package)
        {
            return _mapNameToResourcePackage.TryGetValue(packageName, out package);
        }

        public static void Shutdown()
        {
        }

        public static T LoadAssetSync<T>(string path) where T : UnityEngine.Object
        {
            using var op = YooAssets.LoadAssetSync<T>(path);
            if (op.Status == EOperationStatus.Succeed)
            {
                return op.AssetObject as T;
            }
            else
            {
                PLogger.Error($"{op.LastError}");
                return null;
            }
        }

        public static async UniTask<T> LoadAssetAsync<T>(string path) where T : UnityEngine.Object
        {
            using var op = YooAssets.LoadAssetAsync<T>(path);
            await op.ToUniTask();
            if (op.Status == EOperationStatus.Succeed)
            {
                var asset = op.AssetObject as T;
                return asset;
            }
            else
            {
                PLogger.Error($"{op.LastError}");
                return null;
            }
        }

        public static byte[] LoadRawFileSync(string path)
        {
            using var op = YooAssets.LoadRawFileSync(path);
            if (op.Status == EOperationStatus.Succeed)
            {
                var bytes = op.GetRawFileData();
                return bytes;
            }
            else
            {
                PLogger.Error($"{op.LastError}");
                return null;
            }
        }

        public static async UniTask<byte[]> LoadRawFileAsync(string path)
        {
            using var op = YooAssets.LoadRawFileAsync(path);
            await op.ToUniTask();
            if (op.Status == EOperationStatus.Succeed)
            {
                var bytes = op.GetRawFileData();
                return bytes;
            }
            else
            {
                PLogger.Error($"{op.LastError}");
                return null;
            }
        }

        public static string LoadTextFileSync(string path)
        {
            using var op = YooAssets.LoadRawFileSync(path);
            if (op.Status == EOperationStatus.Succeed)
            {
                var text = op.GetRawFileText();
                return text;
            }
            else
            {
                PLogger.Error($"{op.LastError}");
                return null;
            }
        }

        public static async UniTask<string> LoadTextFileAsync(string path)
        {
            using var op = YooAssets.LoadRawFileAsync(path);
            await op.ToUniTask();
            if (op.Status == EOperationStatus.Succeed)
            {
                var text = op.GetRawFileText();
                return text;
            }
            else
            {
                PLogger.Error($"{op.LastError}");
                return null;
            }
        }


        public static GameObject LoadGameObjectSync(string path, Transform transform = null)
        {
            using var op = YooAssets.LoadAssetSync<GameObject>(path);
            if (op.Status == EOperationStatus.Succeed)
            {
                var go = op.InstantiateSync(transform);
                return go;
            }
            else
            {
                PLogger.Error($"{op.LastError}");
                return null;
            }
        }

        public static async UniTask<GameObject> LoadGameObjectAsync(string path, Transform transform = null)
        {
            using var op = YooAssets.LoadAssetAsync<GameObject>(path);
            await op.ToUniTask();
            if (op.Status == EOperationStatus.Succeed)
            {
                var go = op.InstantiateSync(transform);
                return go;
            }
            else
            {
                PLogger.Error($"{op.LastError}");
                return null;
            }
        }

        public static async UniTask<UnityEngine.SceneManagement.Scene> LoadSceneAsync(string path,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            var op = YooAssets.LoadSceneAsync(path, loadSceneMode);
            await op.ToUniTask();
            if (op.Status == EOperationStatus.Succeed)
            {
                return op.SceneObject;
            }
            else
            {
                PLogger.Error($"{op.LastError}");
                return default;
            }
        }

        public static void UnloadUnusedAssets()
        {
            // assetPkg.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}