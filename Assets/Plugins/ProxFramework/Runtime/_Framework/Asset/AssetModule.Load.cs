using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace ProxFramework.Asset
{
    public static partial class AssetModule
    {
        private static Dictionary<string, HandleBase> _mapLocationToHandle = new();
        private static Dictionary<object, HandleBase> _mapObjectToHandleForRelease = new();
        private static Dictionary<GameObject, object> _mapInstanceObjectToAssetObject = new();
        private static Dictionary<string, SceneHandle> _mapSceneToHandle = new();

        public static bool CheckLocationValid(string location)
        {
            if (!initialized)
            {
#if UNITY_EDITOR
                var asset = AssetDatabase.LoadAssetAtPath<Object>(location);
                return asset != null;
#endif
            }

            foreach (var pkg in _mapNameToResourcePackage.Values)
            {
                if (pkg.CheckLocationValid(location))
                {
                    return true;
                }
            }

            return false;
        }

        public static T GetAssetHandle<T>(string location, bool isAsync, System.Type type = null) where T : HandleBase
        {
            if (_mapLocationToHandle.TryGetValue(location, out var handle))
            {
                if (handle.Status == EOperationStatus.None)
                {
                    _mapLocationToHandle.Remove(location);
                }
                else
                {
                    return handle as T;
                }
            }

            if (!TryGetContainsPackage(location, out var package))
            {
                PLogger.Error($"Cannot find package by location:{location}");
                return null;
            }

            if (typeof(T) == typeof(AssetHandle))
            {
                var assetHandle =
                    isAsync ? package.LoadAssetAsync(location, type) : package.LoadAssetSync(location, type);
                _mapLocationToHandle.Add(location, assetHandle);
                return assetHandle as T;
            }

            if (typeof(T) == typeof(RawFileHandle))
            {
                var rawFileHandle = isAsync ? package.LoadRawFileAsync(location) : package.LoadRawFileSync(location);
                _mapLocationToHandle.Add(location, rawFileHandle);
                return rawFileHandle as T;
            }

            PLogger.Error($"Unsupported Handle Type: {typeof(T).Name}");
            return null;
        }

        public static T LoadAssetSync<T>(string location) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (!initialized)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(location);
                return asset;
            }
#endif
            var assetHandle = GetAssetHandle<AssetHandle>(location, false, typeof(T));
            if (!assetHandle.IsDone)
            {
                assetHandle.WaitForAsyncComplete();
            }

            _mapObjectToHandleForRelease.TryAdd(assetHandle.AssetObject, assetHandle);
            return assetHandle.AssetObject as T;
        }

        public static async UniTask<T> LoadAssetAsync<T>(string location) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (!initialized)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(location);
                return asset;
            }
#endif
            var assetHandle = GetAssetHandle<AssetHandle>(location, true, typeof(T));
            await assetHandle.ToUniTask();
            _mapObjectToHandleForRelease.TryAdd(assetHandle.AssetObject, assetHandle);
            return assetHandle.AssetObject as T;
        }

        public static byte[] LoadRawFileSync(string location)
        {
#if UNITY_EDITOR
            if (!initialized)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(location);
                return asset.bytes;
            }
#endif
            var rawFileHandle = GetAssetHandle<RawFileHandle>(location, false);
            if (!rawFileHandle.IsDone)
            {
                rawFileHandle.WaitForAsyncComplete();
            }

            return rawFileHandle.GetRawFileData();
        }

        public static async UniTask<byte[]> LoadRawDataAsync(string location)
        {
#if UNITY_EDITOR
            if (!initialized)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(location);
                return asset.bytes;
            }
#endif
            var rawFileHandle = GetAssetHandle<RawFileHandle>(location, true);
            await rawFileHandle.ToUniTask();
            return rawFileHandle.GetRawFileData();
        }

        public static string LoadTextFileSync(string location)
        {
#if UNITY_EDITOR
            if (!initialized)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(location);
                return asset.text;
            }
#endif
            var rawFileHandle = GetAssetHandle<RawFileHandle>(location, false);
            if (!rawFileHandle.IsDone)
            {
                rawFileHandle.WaitForAsyncComplete();
            }

            return rawFileHandle.GetRawFileText();
        }

        public static async UniTask<string> LoadTextFileAsync(string location)
        {
#if UNITY_EDITOR
            if (!initialized)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(location);
                return asset.text;
            }
#endif
            var rawFileHandle = GetAssetHandle<RawFileHandle>(location, true);
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

            var handle = package.LoadSceneAsync(location, loadSceneMode);
            await handle.ToUniTask();
            if (!_mapSceneToHandle.TryAdd(handle.SceneObject.name, handle))
            {
                handle.Release();
            }

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
                handle.Release();
                _mapSceneToHandle.Remove(sceneName);
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

            if (_mapObjectToHandleForRelease.Remove(assetObject, out var handle))
            {
                _mapLocationToHandle.Remove(handle.GetAssetInfo().AssetPath);
                handle.Release();
            }
        }

        public static void ReleaseAllHandles()
        {
            foreach (var handle in _mapLocationToHandle.Values)
            {
                handle.Release();
            }

            _mapLocationToHandle.Clear();
            _mapObjectToHandleForRelease.Clear();
            _mapInstanceObjectToAssetObject.Clear();
            _mapSceneToHandle.Clear();
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