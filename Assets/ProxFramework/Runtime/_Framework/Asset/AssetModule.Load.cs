using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace ProxFramework.Asset
{
    public static partial class AssetModule
    {
        private static Dictionary<string, HandleBase> _mapLocationToHandle = new();
        private static Dictionary<object, HandleBase> _mapObjectToHandle = new();
        private static Dictionary<GameObject, object> _mapInstanceObjectToAssetObject = new();
        private static Dictionary<string, SceneHandle> _mapSceneToHandle = new();

        private static T GetHandle<T>(string location, bool isAsync) where T : HandleBase
        {
            if (_mapLocationToHandle.TryGetValue(location, out var handle))
            {
                return handle as T;
            }

            if (!TryGetContainsPackage(location, out var package))
            {
                return null;
            }

            if (typeof(T) == typeof(AssetHandle))
            {
                var assetHandle = isAsync ? package.LoadAssetAsync(location) : package.LoadAssetSync(location);
                _mapLocationToHandle.Add(location, assetHandle);
                return assetHandle as T;
            }

            if (typeof(T) == typeof(RawFileHandle))
            {
                var rawFileHandle = isAsync ? package.LoadRawFileAsync(location) : package.LoadRawFileSync(location);
                _mapLocationToHandle.Add(location, rawFileHandle);
                return rawFileHandle as T;
            }

            return null;
        }

        public static T LoadAssetSync<T>(string location) where T : UnityEngine.Object
        {
            var assetHandle = GetHandle<AssetHandle>(location, false);
            if (!assetHandle.IsDone)
            {
                assetHandle.WaitForAsyncComplete();
            }

            _mapObjectToHandle.TryAdd(assetHandle.AssetObject, assetHandle);
            return assetHandle.AssetObject as T;
        }

        public static async UniTask<T> LoadAssetAsync<T>(string location) where T : UnityEngine.Object
        {
            var assetHandle = GetHandle<AssetHandle>(location, true);
            await assetHandle.ToUniTask();
            _mapObjectToHandle.TryAdd(assetHandle.AssetObject, assetHandle);
            return assetHandle.AssetObject as T;
        }

        public static byte[] LoadRawFileSync(string location)
        {
            var rawFileHandle = GetHandle<RawFileHandle>(location, false);
            if (!rawFileHandle.IsDone)
            {
                rawFileHandle.WaitForAsyncComplete();
            }

            return rawFileHandle.GetRawFileData();
        }

        public static async UniTask<byte[]> LoadRawDataAsync(string location)
        {
            var rawFileHandle = GetHandle<RawFileHandle>(location, true);
            await rawFileHandle.ToUniTask();
            return rawFileHandle.GetRawFileData();
        }

        public static string LoadTextFileSync(string location)
        {
            var rawFileHandle = GetHandle<RawFileHandle>(location, false);
            if (!rawFileHandle.IsDone)
            {
                rawFileHandle.WaitForAsyncComplete();
            }

            return rawFileHandle.GetRawFileText();
        }

        public static async UniTask<string> LoadTextFileAsync(string location)
        {
            var rawFileHandle = GetHandle<RawFileHandle>(location, true);
            await rawFileHandle.ToUniTask();
            return rawFileHandle.GetRawFileText();
        }

        public static GameObject InstantiateGameObjectSync(string location, Transform parentTransform = null,
            bool stayWorld = false)
        {
            var assetGameObject = LoadAssetSync<GameObject>(location);
            if (assetGameObject == null)
            {
                return null;
            }

            var instantiatedGameObject = UnityEngine.Object.Instantiate(assetGameObject, parentTransform, stayWorld);
            _mapInstanceObjectToAssetObject.Add(instantiatedGameObject, assetGameObject);
            return instantiatedGameObject;
        }

        public static async UniTask<GameObject> InstantiateGameObjectAsync(string location,
            Transform parentTransform = null)
        {
            var result = await InstantiateMultiGameObjectAsync(location, 1, parentTransform);
            if (result == null || result.Length == 0)
            {
                return null;
            }

            return result[0];
        }

        public static async UniTask<GameObject[]> InstantiateMultiGameObjectAsync(string path, int count = 1,
            Transform transform = null)
        {
            var gameObject = await LoadAssetAsync<GameObject>(path);
            if (gameObject == null)
            {
                return null;
            }

            var op = UnityEngine.Object.InstantiateAsync(gameObject, count, transform);
            await op.ToUniTask();
            foreach (var gameObj in op.Result)
            {
                _mapInstanceObjectToAssetObject.Add(gameObj, gameObject);
            }

            return op.Result;
        }

        public static async UniTask<UnityEngine.SceneManagement.Scene> LoadSceneAsync(string location,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (!TryGetContainsPackage(location, out var package))
            {
                PLogger.Error($"Cannot find package by location:{location}");
                return new UnityEngine.SceneManagement.Scene();
            }

            var sceneName = Path.GetFileNameWithoutExtension(location);
            if (_mapSceneToHandle.ContainsKey(sceneName))
            {
                PLogger.Warning($"Scene {sceneName} already loaded.");
                return new UnityEngine.SceneManagement.Scene();
            }

            var handle = package.LoadSceneAsync(location, loadSceneMode);
            await handle.ToUniTask();
            _mapSceneToHandle.Add(sceneName, handle);
            return handle.SceneObject;
        }

        public static async UniTask UnloadSceneAsync(UnityEngine.SceneManagement.Scene scene)
        {
            await UnloadSceneAsync(scene.name);
        }

        public static async UniTask UnloadSceneAsync(string sceneName)
        {
            if (_mapSceneToHandle.TryGetValue(sceneName, out var handle))
            {
                await handle.UnloadAsync();
                _mapSceneToHandle.Remove(sceneName);
            }
        }

        public static void ReleaseAsset(string location)
        {
            if (_mapLocationToHandle.TryGetValue(location, out var handle))
            {
                handle.Release();
                _mapLocationToHandle.Remove(location);
            }
            else
            {
                PLogger.Warning($"Cannot find package by location:{location}");
            }
        }

        public static void ReleaseGameObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            UnityEngine.Object.Destroy(gameObject);
            if (_mapInstanceObjectToAssetObject.TryGetValue(gameObject, out var assetObject))
            {
                ReleaseAsset(assetObject);
            }
        }

        public static void ReleaseAsset(object assetObject)
        {
            if (assetObject == null)
            {
                return;
            }

            if (_mapObjectToHandle.TryGetValue(assetObject, out var handle))
            {
                handle.Release();
                _mapObjectToHandle.Remove(assetObject);
            }
        }

        public static async UniTask UnloadUnusedAssets()
        {
            foreach (var pkg in _mapNameToResourcePackage.Values)
            {
                await pkg.UnloadUnusedAssetsAsync();
            }

            PLogger.Info($"UnloadUnusedAssets completed.");
        }

        public static async UniTask ForceUnloadAllAssets()
        {
            foreach (var pkg in _mapNameToResourcePackage.Values)
            {
                await pkg.UnloadAllAssetsAsync();
            }

            PLogger.Info($"UnloadAllAssets completed.");
        }
    }
}