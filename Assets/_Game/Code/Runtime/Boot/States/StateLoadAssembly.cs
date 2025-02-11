using System;
using System.IO;
using Cysharp.Threading.Tasks;
using ProxFramework;
using ProxFramework.Asset;
using ProxFramework.Runtime.Settings;
using ProxFramework.StateMachine;

#if ENABLE_HCLR
#endif

namespace GameName.Runtime
{
    public class StateLoadAssembly : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            PLogger.Info("StateLoadAssembly");
            if (SettingsUtil.GlobalSettings.hclrSettings.Enable)
            {
            }
            else
            {
            }
        }

        public override void Exit()
        {
        }

        /// <summary>
        /// 为Aot Assembly加载原始metadata， 这个代码放Aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行。
        /// </summary>
        public async UniTaskVoid LoadMetadataForAOTAssembly()
        {
            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
            // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

            // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            if (SettingsUtil.GlobalSettings.hclrSettings.aotMetaAssemblies.Length == 0)
            {
                // m_LoadMetadataAssemblyComplete = true;
                return;
            }

            foreach (string aotDllName in SettingsUtil.GlobalSettings.hclrSettings.aotMetaAssemblies)
            {
                var assetLocation = Path.Combine("Assets",
                    SettingsUtil.GlobalSettings.hclrSettings.assemblyBytesAssetDir,
                    $"{aotDllName}{SettingsUtil.GlobalSettings.hclrSettings.assemblyBytesAssetExtension}");
                PLogger.Info($"LoadMetadataAsset: [ {assetLocation} ]");
                var bytes = await AssetModule.LoadRawDataAsync(assetLocation);

                try
                {
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}