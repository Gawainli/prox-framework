using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace ProxFramework.Asset
{
    public static partial class AssetModule
    {
        private static Dictionary<string, AssetHandle> _mapLocationToAssetHandle = new();
        private static Dictionary<string, RawFileHandle> _mapLocationToRawFileHandle = new();
        private static Dictionary<object, HandleBase> _mapAssetObjectToHandle = new();

        public static T LoadAssetSync<T>(string location) where T : UnityEngine.Object
        {
            var handle = _mapLocationToAssetHandle.GetValueOrDefault(location);
            if (handle == null)
            {
                if (!TryGetContainsPackage(location, out var package))
                {
                    return null;
                }

                handle = package.LoadAssetSync(location);
                _mapLocationToAssetHandle.Add(location, handle);
            }

            if (!handle.IsDone)
            {
                handle.WaitForAsyncComplete();
            }

            return handle.AssetObject as T;
        }

        public static async UniTask<T> LoadAssetAsync<T>(string location)
            where T : UnityEngine.Object
        {
            var handle = _mapLocationToAssetHandle.GetValueOrDefault(location);
            if (handle == null)
            {
                if (!TryGetContainsPackage(location, out var package))
                {
                    return null;
                }
            
                handle = package.LoadAssetAsync(location);
                _mapLocationToAssetHandle.Add(location, handle);
            }
            
            await handle.ToUniTask();
            _mapAssetObjectToHandle.TryAdd(handle.AssetObject, handle);
            return handle.AssetObject as T;

            // if (!TryGetContainsPackage(location, out var package))
            // {
            //     return null;
            // }
            //
            // using var handle = package.LoadAssetAsync<T>(location);
            // await UniTask.WaitForSeconds(1);
            // await handle.ToUniTask();
            // return handle.AssetObject as T;
        }

        public static byte[] LoadRawFileSync(string location)
        {
            var handle = _mapLocationToRawFileHandle.GetValueOrDefault(location);
            if (handle == null)
            {
                if (!TryGetContainsPackage(location, out var package))
                {
                    return null;
                }

                handle = package.LoadRawFileSync(location);
                _mapLocationToRawFileHandle.Add(location, handle);
            }

            if (!handle.IsDone)
            {
                handle.WaitForAsyncComplete();
            }

            return handle.GetRawFileData();
        }

        public static async UniTask<byte[]> LoadRawDataAsync(string location)
        {
            var handle = _mapLocationToRawFileHandle.GetValueOrDefault(location);
            if (handle == null)
            {
                if (!TryGetContainsPackage(location, out var package))
                {
                    return null;
                }

                handle = package.LoadRawFileAsync(location);
                _mapLocationToRawFileHandle.Add(location, handle);
            }

            await handle.ToUniTask();
            return handle.GetRawFileData();
        }

        public static string LoadTextFileSync(string location)
        {
            var handle = _mapLocationToRawFileHandle.GetValueOrDefault(location);
            if (handle == null)
            {
                if (!TryGetContainsPackage(location, out var package))
                {
                    return null;
                }

                handle = package.LoadRawFileSync(location);
                _mapLocationToRawFileHandle.Add(location, handle);
            }

            if (!handle.IsDone)
            {
                handle.WaitForAsyncComplete();
            }

            return handle.GetRawFileText();
        }

        public static async UniTask<string> LoadTextFileAsync(string location)
        {
            var handle = _mapLocationToRawFileHandle.GetValueOrDefault(location);
            if (handle == null)
            {
                if (!TryGetContainsPackage(location, out var package))
                {
                    return null;
                }

                handle = package.LoadRawFileAsync(location);
                _mapLocationToRawFileHandle.Add(location, handle);
            }

            await handle.ToUniTask();
            return handle.GetRawFileText();
        }


        public static GameObject InstantiateGameObjectSync(string location, Transform parentTransform = null,
            bool stayWorld = false)
        {
            var gameObject = LoadAssetSync<GameObject>(location);
            return gameObject == null ? null : UnityEngine.Object.Instantiate(gameObject, parentTransform, stayWorld);
        }

        public static async UniTask<GameObject> InstantiateGameObjectAsync(string path, Transform transform = null)
        {
            var gameObject = await LoadAssetAsync<GameObject>(path);
            return gameObject == null ? null : UnityEngine.Object.Instantiate(gameObject, transform);
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
            PLogger.Info($"LoadSceneAsync {location} success.");
            return handle.SceneObject;
        }

        public static void ReleaseAsset(string location)
        {
            var handle = _mapLocationToAssetHandle.GetValueOrDefault(location);
            if (handle == null)
            {
                PLogger.Warning($"Cannot find package by location:{location}");
                return;
            }
            handle.Release();
            _mapLocationToAssetHandle.Remove(location);
        }

        public static void ReleaseAsset(object assetObject)
        {
            var handle = _mapAssetObjectToHandle.GetValueOrDefault(assetObject);
            handle?.Release();
            _mapAssetObjectToHandle.Remove(assetObject);
        }

        public static async UniTask UnloadUnusedAssets()
        {
            foreach (var pkg in _mapNameToResourcePackage.Values)
            {
                await pkg.UnloadUnusedAssetsAsync();
                // await pkg.UnloadAllAssetsAsync();
            }
            // await Resources.UnloadUnusedAssets();
            PLogger.Info($"UnloadUnusedAssets completed.");
        }
    }
}