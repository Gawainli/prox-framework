using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using ProxFramework.Logger;
using ProxFramework.Module;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace ProxFramework.Asset
{
    public class ResLogger : YooAsset.ILogger
    {
        public void Log(string message)
        {
            LogModule.Info(message);
        }

        public void Warning(string message)
        {
            LogModule.Warning(message);
        }

        public void Error(string message)
        {
            LogModule.Error(message);
        }

        public void Exception(Exception exception)
        {
            LogModule.Exception(exception.ToString());
        }
    }

    public class AssetModule : IModule
    {
        public static AssetModuleCfg cfg;
        public static ResourcePackage assetPkg;
        public static ResourcePackage rawPkg;
        public static string packageVersion;

        public static DownloaderOperation downloaderOperation;
        public static int downloadingMaxNum = 10;
        public static int failedTryAgain = 3;

        private class GameDecryptionServices : IDecryptionServices
        {
            public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
            {
                return 32;
            }

            public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
            {
                throw new NotImplementedException();
            }

            public Stream LoadFromStream(DecryptFileInfo fileInfo)
            {
                var bundleStream =
                    new FileStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return bundleStream;
            }

            public uint GetManagedReadBufferSize()
            {
                return 1024;
            }

            public AssetBundle LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                throw new NotImplementedException();
            }

            public AssetBundleCreateRequest LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                throw new NotImplementedException();
            }
        }

        public void Initialize(object userData = null)
        {
            if (userData == null)
            {
                LogModule.Error("ResourceModule Initialize failed");
                return;
            }

            cfg = userData as AssetModuleCfg;
            if (cfg == null || string.IsNullOrEmpty(cfg.assetPkgName))
            {
                LogModule.Error("ResourceModule Initialize failed");
                return;
            }

            YooAssets.Initialize(new ResLogger());
            assetPkg = YooAssets.TryGetPackage(cfg.assetPkgName);
            if (assetPkg == null)
            {
                assetPkg = YooAssets.CreatePackage(cfg.assetPkgName);
                YooAssets.SetDefaultPackage(assetPkg);
            }

            if (!String.IsNullOrEmpty(cfg.rawPkgName))
            {
                rawPkg = YooAssets.TryGetPackage(cfg.rawPkgName);
                if (rawPkg == null)
                {
                    rawPkg = YooAssets.CreatePackage(cfg.rawPkgName);
                }
            }
        }

        public void Tick(float deltaTime, float unscaledDeltaTime)
        {
        }

        public void Shutdown()
        {
        }

        public int Priority { get; set; }
        public bool Initialized { get; set; }

        public async UniTask<bool> InitPkgAsync(ResourcePackage pkg, string pkgName)
        {
            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (cfg.ePlayMode == EPlayMode.EditorSimulateMode)
            {
                var createParameters = new EditorSimulateModeParameters
                {
                    SimulateManifestFilePath =
                        EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline.ToString(),
                            pkgName)
                };
                initializationOperation = pkg.InitializeAsync(createParameters);
            }

            // 单机运行模式
            if (cfg.ePlayMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters
                {
                    DecryptionServices = new GameDecryptionServices(),
                };
                initializationOperation = pkg.InitializeAsync(createParameters);
            }

            // 联机运行模式
            if (cfg.ePlayMode == EPlayMode.HostPlayMode)
            {
                var createParameters = new HostPlayModeParameters
                {
                    DecryptionServices = new GameDecryptionServices(),
                    // createParameters.QueryServices = new GameQueryServices();
                    RemoteServices = new YooRemoteService(cfg.DefaultHostServer, cfg.DefaultHostServer)
                };
                initializationOperation = pkg.InitializeAsync(createParameters);
            }

            //webgl模式运行
            if (cfg.ePlayMode == EPlayMode.WebPlayMode)
            {
                var createParameters = new WebPlayModeParameters
                {
                    DecryptionServices = new GameDecryptionServices(),
                    RemoteServices = new YooRemoteService(cfg.DefaultHostServer, cfg.DefaultHostServer),
                    BuildinQueryServices = new GameQueryServices(),
                };
                initializationOperation = pkg.InitializeAsync(createParameters);
                YooAssets.SetCacheSystemDisableCacheOnWebGL();
            }

            if (initializationOperation == null)
            {
                LogModule.Error("ResourceModule Initialize failed");
                return false;
            }

            await initializationOperation.ToUniTask();
            if (initializationOperation.Status == EOperationStatus.Succeed)
            {
                LogModule.Info("AssetModule Initialize Succeed");
                Initialized = true;
                return true;
            }
            else
            {
                LogModule.Error($"{initializationOperation.Error}");
                return false;
            }
        }

        public async UniTask<bool> InitPkgAsync()
        {
            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (cfg.ePlayMode == EPlayMode.EditorSimulateMode)
            {
                var createParameters = new EditorSimulateModeParameters
                {
                    SimulateManifestFilePath =
                        EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline.ToString(),
                            cfg.assetPkgName)
                };
                initializationOperation = assetPkg.InitializeAsync(createParameters);
            }

            // 单机运行模式
            if (cfg.ePlayMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters
                {
                    DecryptionServices = new GameDecryptionServices()
                };
                initializationOperation = assetPkg.InitializeAsync(createParameters);
            }

            // 联机运行模式
            if (cfg.ePlayMode == EPlayMode.HostPlayMode)
            {
                var createParameters = new HostPlayModeParameters
                {
                    DecryptionServices = new GameDecryptionServices(),
                    // createParameters.QueryServices = new GameQueryServices();
                    RemoteServices = new YooRemoteService(cfg.DefaultHostServer, cfg.DefaultHostServer)
                };
                initializationOperation = assetPkg.InitializeAsync(createParameters);
            }

            //webgl模式运行
            if (cfg.ePlayMode == EPlayMode.WebPlayMode)
            {
                var createParameters = new WebPlayModeParameters
                {
                    DecryptionServices = new GameDecryptionServices(),
                    RemoteServices = new YooRemoteService(cfg.DefaultHostServer, cfg.DefaultHostServer),
                    BuildinQueryServices = new GameQueryServices(),
                };
                initializationOperation = assetPkg.InitializeAsync(createParameters);
            }

            if (initializationOperation == null)
            {
                LogModule.Error("ResourceModule Initialize failed");
                return false;
            }

            await initializationOperation.ToUniTask();
            if (initializationOperation.Status == EOperationStatus.Succeed)
            {
                LogModule.Info("AssetModule Initialize Succeed");
                Initialized = true;
                return true;
            }
            else
            {
                LogModule.Error($"{initializationOperation.Error}");
                return false;
            }
        }

        public static async UniTask<bool> GetStaticVersion()
        {
            var op = assetPkg.UpdatePackageVersionAsync();
            await op.ToUniTask();
            if (op.Status == EOperationStatus.Succeed)
            {
                LogModule.Info($"GetStaticVersion Succeed. version: {op.PackageVersion}");
                packageVersion = op.PackageVersion;
                return true;
            }
            else
            {
                LogModule.Error($"{op.Error}");
                return false;
            }
        }

        public static async UniTask<bool> UpdateManifest()
        {
            var op = assetPkg.UpdatePackageManifestAsync(packageVersion, true);
            await op.ToUniTask();

            if (op.Status == EOperationStatus.Succeed)
            {
                LogModule.Info($"UpdateManifest Succeed.");
                return true;
            }
            else
            {
                LogModule.Error($"{op.Error}");
                return false;
            }
        }

        public static bool CreateDownloader()
        {
            var downloader = YooAssets.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            var totalDownloadCount = downloader.TotalDownloadCount;
            var totalDownloadBytes = downloader.TotalDownloadBytes;

            if (downloader.TotalDownloadCount == 0)
            {
                LogModule.Info("No files need to download");
                return false;
            }
            else
            {
                LogModule.Info(
                    $"Found {totalDownloadCount} files need to download, total size is {totalDownloadBytes} bytes");
                downloaderOperation = downloader;
                return true;
            }
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
                LogModule.Error($"{op.LastError}");
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
                LogModule.Error($"{op.LastError}");
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
                LogModule.Error($"{op.LastError}");
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
                LogModule.Error($"{op.LastError}");
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
                LogModule.Error($"{op.LastError}");
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
                LogModule.Error($"{op.LastError}");
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
                LogModule.Error($"{op.LastError}");
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
                LogModule.Error($"{op.LastError}");
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
                LogModule.Error($"{op.LastError}");
                return default;
            }
        }

        public static void UnloadUnusedAssets()
        {
            assetPkg.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}