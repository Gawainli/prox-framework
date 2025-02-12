using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using HybridCLR;
using ProxFramework;
using ProxFramework.Asset;
using ProxFramework.Runtime.Settings;

namespace Prox.GameName123.Runtime
{
    public static class GameAssembly
    {
        private static readonly ConcurrentDictionary<string, Assembly> _hotUpdateAssemblies = new();
        private static readonly ConcurrentDictionary<string, Type> _cachedTypes = new();

        public static async UniTask LoadHotUpdateAssemblies()
        {
            if (!SettingsUtil.GlobalSettings.hclrSettings.Enable) return;

            foreach (var hotUpdateDllName in SettingsUtil.GlobalSettings.hclrSettings.hotUpdateAssemblies)
            {
                var assetLocation = Path.Combine(SettingsUtil.GlobalSettings.hclrSettings.assemblyBytesAssetDir,
                    $"{hotUpdateDllName}{SettingsUtil.GlobalSettings.hclrSettings.assemblyBytesAssetExtension}");
                var bytes = await AssetModule.LoadRawDataAsync(assetLocation);
                LoadBytes(bytes);
            }
        }

        /// <summary>
        /// 为Aot Assembly加载原始metadata， 这个代码放Aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行。
        /// </summary>
        public static async UniTaskVoid LoadMetadataForAOTAssembly()
        {
            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
            // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

            // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            if (SettingsUtil.GlobalSettings.hclrSettings.aotMetaAssemblies.Length == 0) return;

            foreach (string aotDllName in SettingsUtil.GlobalSettings.hclrSettings.aotMetaAssemblies)
            {
                var assetLocation = Path.Combine(SettingsUtil.GlobalSettings.hclrSettings.assemblyBytesAssetDir,
                    $"{aotDllName}{SettingsUtil.GlobalSettings.hclrSettings.assemblyBytesAssetExtension}");
                PLogger.Info($"LoadMetadataAsset: [ {assetLocation} ]");
                var bytes = await AssetModule.LoadRawDataAsync(assetLocation);

                try
                {
                    var errorCode = RuntimeApi.LoadMetadataForAOTAssembly(bytes, HomologousImageMode.SuperSet);
                    PLogger.Info($"LoadMetadataForAOTAssembly: {aotDllName} errorCode: {errorCode}");
                }
                catch (Exception e)
                {
                    PLogger.Error($"LoadMetadataForAOTAssembly failed: {e}");
                    throw;
                }
            }
        }

        public static void LoadBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                PLogger.Error("LoadAssembly failed, bytes is null or empty");
                return;
            }

            try
            {
                var assembly = Assembly.Load(bytes);
                if (!_hotUpdateAssemblies.TryAdd(assembly.FullName, assembly))
                {
                    PLogger.Warning($"Assembly {assembly.FullName} is already loaded");
                    return;
                }

                foreach (var type in assembly.GetExportedTypes())
                {
                    if (!_cachedTypes.TryAdd(type.FullName, type))
                    {
                        PLogger.Warning($"Type {type.FullName} is already cached");
                    }
                }
            }
            catch (Exception e)
            {
                PLogger.Error($"LoadAssembly failed: {e}");
                throw;
            }
        }

        public static T CreateInstance<T>(object[] args = null)
        {
            args ??= Array.Empty<object>();

            try
            {
                var type = GetTypeFromCacheOrDefault(typeof(T).FullName);
                if (type == null)
                {
                    PLogger.Error($"Type {typeof(T).FullName} not found in cached types or default assemblies");
                    return default;
                }

                return (T)Activator.CreateInstance(type, args);
            }
            catch (MissingMethodException ex)
            {
                PLogger.Error($"Constructor not found for type {typeof(T).FullName}: {ex.Message}");
                throw;
            }
            catch (Exception e)
            {
                PLogger.Error($"CreateInstance failed: {e}");
                throw;
            }
        }

        public static object CallStatic(string typeFullName, string funcName, object[] args = null)
        {
            if (string.IsNullOrEmpty(typeFullName) || string.IsNullOrEmpty(funcName))
            {
                throw new ArgumentNullException("Type name and method name cannot be null or empty.");
            }

            args ??= Array.Empty<object>();

            try
            {
                var type = GetTypeFromCacheOrDefault(typeFullName);
                if (type == null)
                {
                    PLogger.Error($"Type {typeFullName} not found in cached types or default assemblies");
                    throw new TypeLoadException($"Type {typeFullName} not found in cached types or default assemblies");
                }

                var method = type.GetMethod(funcName, BindingFlags.Static | BindingFlags.Public, null,
                    args.Select(a => a?.GetType() ?? typeof(object)).ToArray(), null);

                if (method == null)
                {
                    PLogger.Error($"Method {funcName} not found in type {type.FullName}");
                    throw new MissingMethodException($"Method {funcName} not found in type {type.FullName}");
                }

                return method.Invoke(null, args);
            }
            catch (TargetInvocationException ex)
            {
                PLogger.Error($"Method invocation failed: {ex.InnerException?.Message}");
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                PLogger.Error($"CallStatic failed: {ex.Message}");
                throw;
            }
        }

        public static object CallStatic<T>(string funcName, object[] args = null)
        {
            return CallStatic(typeof(T).FullName, funcName, args);
        }

        private static Type GetTypeFromCacheOrDefault(string typeFullName)
        {
            if (_cachedTypes.TryGetValue(typeFullName, out var type))
            {
                return type;
            }

            PLogger.Info($"Type: {typeFullName} not found in cache, try to get from default assemblies");
            type = Type.GetType(typeFullName) ??
                   Assembly.GetExecutingAssembly().GetType(typeFullName) ??
                   Assembly.GetCallingAssembly().GetType(typeFullName);

            if (type == null)
            {
                foreach (var hotUpdateName in SettingsUtil.GlobalSettings.hclrSettings.hotUpdateAssemblies)
                {
                    var assembly = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => a.GetName().Name == hotUpdateName);
                    if (assembly == null) continue;
                    type = assembly.GetType(typeFullName);
                    if (type != null)
                    {
                        break;
                    }
                }
            }

            if (type != null)
            {
                _cachedTypes.TryAdd(typeFullName, type);
            }

            return type;
        }
    }
}