using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ProxFramework.Asset;

using UnityEngine;
using YooAsset;

namespace ProxFramework.Pool
{
    public class AssetObjectPool
    {
        private readonly GameObject _root;
        private GameObject _prefab;
        private readonly Queue<GameObject> _cacheObjects;
        private readonly bool _dontDestroy;
        private readonly int _initCapacity;
        private readonly int _maxCapacity;
        private readonly float _destroyTime;
        private float _lastRestoreRealTime = -1f;

        public string AssetPath { private set; get; }

        public int CacheCount
        {
            get { return _cacheObjects.Count; }
        }

        public int SpawnCount { private set; get; } = 0;

        public bool DontDestroy
        {
            get { return _dontDestroy; }
        }

        public AssetObjectPool(GameObject poolingRoot, string assetPath, bool dontDestroy, int initCapacity,
            int maxCapacity, float destroyTime)
        {
            _root = new GameObject(assetPath);
            _root.transform.parent = poolingRoot.transform;
            AssetPath = assetPath;

            _dontDestroy = dontDestroy;
            _initCapacity = initCapacity;
            _maxCapacity = maxCapacity;
            _destroyTime = destroyTime;

            _cacheObjects = new Queue<GameObject>(initCapacity);
        }

        public void CreatePoolSync(ResourcePackage package)
        {
            _prefab = AssetModule.LoadAssetSync<GameObject>(AssetPath);

            for (int i = 0; i < _initCapacity; i++)
            {
                var go = Object.Instantiate(_prefab, _root.transform);
                go.SetActive(false);
                _cacheObjects.Enqueue(go);
            }
        }

        public async UniTask CreatePoolAsync(ResourcePackage package)
        {
            if (package == null)
            {
                PLogger.Error("CreatePoolAsync package is null");
                return;
            }

            _prefab = await AssetModule.LoadAssetAsync<GameObject>(AssetPath);

            for (int i = 0; i < _initCapacity; i++)
            {
                var go = Object.Instantiate(_prefab, _root.transform);
                go.SetActive(false);
                _cacheObjects.Enqueue(go);
            }
        }

        public void DestroyPool()
        {
            Object.Destroy(_prefab);
            Object.Destroy(_root);
            _cacheObjects.Clear();
            SpawnCount = 0;
        }

        public bool CanAutoDestroy()
        {
            if (_dontDestroy)
                return false;
            if (_destroyTime < 0)
                return false;

            if (_lastRestoreRealTime > 0 && SpawnCount <= 0)
                return (Time.realtimeSinceStartup - _lastRestoreRealTime) > _destroyTime;
            else
                return false;
        }

        public void Restore(GameObject poolObj)
        {
            SpawnCount--;
            if (SpawnCount <= 0)
                _lastRestoreRealTime = Time.realtimeSinceStartup;

            // 如果缓存池还未满员
            if (_cacheObjects.Count < _maxCapacity)
            {
                SetRestoreGameObject(poolObj);
                _cacheObjects.Enqueue(poolObj);
            }
            else
            {
                Object.Destroy(poolObj);
            }
        }

        public void Discard(GameObject poolObj)
        {
            SpawnCount--;
            if (SpawnCount <= 0)
                _lastRestoreRealTime = Time.realtimeSinceStartup;
            GameObject.Destroy(poolObj);
        }

        public GameObject Spawn()
        {
            return Spawn(null, Vector3.zero, Quaternion.identity, false, null);
        }

        public GameObject Spawn(Transform parent, Vector3 position, Quaternion rotation, bool forceClone,
            params System.Object[] userDatas)
        {
            GameObject go = null;
            if (forceClone == false && _cacheObjects.Count > 0)
                go = _cacheObjects.Dequeue();
            else
                go = GameObject.Instantiate(_prefab, _root.transform);

            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.parent = parent;
            var poolObj = go.GetComponent<IPoolObj>();
            if (poolObj != null)
            {
                poolObj.Init(userDatas);
                poolObj.Pool = this;
            }
            SpawnCount++;
            return go;
        }
        
        private void SetRestoreGameObject(GameObject go)
        {
            if (go != null)
            {
                go.SetActive(false);
                go.transform.SetParent(_root.transform);
                go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                var poolObj = go.GetComponent<IPoolObj>();
                if (poolObj != null)
                {
                    poolObj.Reset();
                }
            }
        }
    }
}