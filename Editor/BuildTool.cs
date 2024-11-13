using System;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace ProxFramework.Editor
{
    public static class BuildTool
    {
        // private static readonly string _hotUpdateDllsPath = "Assets/_GameMain/HotUpdateDll";

        // [MenuItem("Build Tool/Build All (all hclr and all assets)")]
        public static void BuildAll()
        {
#if HCLR
	        HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
            CopyAllAOTAndHotUpdate();
#endif
            BuildYooAssets(EditorUserBuildSettings.activeBuildTarget);
        }

        private static void BuildYooAssets(BuildTarget target, EBuildMode buildMode = EBuildMode.ForceRebuild)
        {
            Debug.Log($"开始构建 : {target}");
            BuildParameters buildParameters = new BuiltinBuildParameters();
            // buildParameters.StreamingAssetsRoot = AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot();
            buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            // buildParameters.BuildPipeline = AssetBundleBuilderSetting.GetPackageBuildMode(AssetBundleBuilderSettingData.Setting.BuildPackage, BuildPipeline);
            // buildParameters.BuildTarget = target;
            // buildParameters.BuildMode = buildMode;
            // buildParameters.PackageName = AssetBundleBuilderSettingData.Setting.BuildPackage;
            // buildParameters.PackageVersion = GetBuildPackageVersion();
            // buildParameters.VerifyBuildingResult = true;
            // buildParameters.SharedPackRule = new ZeroRedundancySharedPackRule();
            // // buildParameters.EncryptionServices = CreateEncryptionServicesInstance();
            // buildParameters.CompressOption = AssetBundleBuilderSettingData.Setting.CompressOption;
            // buildParameters.OutputNameStyle = AssetBundleBuilderSettingData.Setting.OutputNameStyle;
            // buildParameters.CopyBuildinFileOption = AssetBundleBuilderSettingData.Setting.CopyBuildinFileOption;
            // buildParameters.CopyBuildinFileTags = AssetBundleBuilderSettingData.Setting.CopyBuildinFileTags;
            //
            // if (AssetBundleBuilderSettingData.Setting.BuildPipeline == EBuildPipeline.ScriptableBuildPipeline)
            // {
            // 	buildParameters.SBPParameters = new BuildParameters.SBPBuildParameters();
            // 	buildParameters.SBPParameters.WriteLinkXML = true;
            // }
            //
            // var builder = new AssetBundleBuilder();
            // var buildResult = builder.Run(buildParameters);
            // if (buildResult.Success)
            // {
            // 	EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
            // }
        }

        private static string GetBuildPackageVersion()
        {
            int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
        }
        
#region HCLR
#if HCLR
        [MenuItem("Build Tool/Copy All AOT and HotUpdate DLLs")]
        public static void CopyAllAOTAndHotUpdate()
        {
            CopyAllAotDLLs(EditorUserBuildSettings.activeBuildTarget);
            CopyAllHotUpdateDLLs(EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
        }

        [MenuItem("Build Tool/Build Fast (hotupdate and incr assets)")]
        public static void BuildAllIncremental()
        {
            HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTarget();
            CopyAllAOTAndHotUpdate();
            BuildYooAssets(EditorUserBuildSettings.activeBuildTarget, EBuildMode.IncrementalBuild);
        }

        private static void CheckHotUpdateDllsPath()
        {
            var path = Path.Combine(HybridCLR.Editor.SettingsUtil.ProjectDir, _hotUpdateDllsPath);
            if (!Directory.Exists(_hotUpdateDllsPath))
            {
                Directory.CreateDirectory(_hotUpdateDllsPath);
            }
        }

        private static void CopyAllAotDLLs(BuildTarget target)
        {
            CheckHotUpdateDllsPath();
            var aotDllsPath = HybridCLR.Editor.SettingsUtil.AssembliesPostIl2CppStripDir;
            var projDir = HybridCLR.Editor.SettingsUtil.ProjectDir;
            foreach (var aotName in HybridCLR.Editor.SettingsUtil.AOTAssemblyNames)
            {
                var srcPath = aotDllsPath + "/" + target.ToString() + "/" + aotName + ".dll";
                var dstPath = _hotUpdateDllsPath + "/" + aotName + ".dll.bytes";
                srcPath = Path.Combine(projDir, srcPath);
                dstPath = Path.Combine(projDir, dstPath);
                Debug.Log($"Copy Aot Dll {srcPath} to {dstPath}");
                File.Copy(srcPath, dstPath, true);
            }
        }

        private static void CopyAllHotUpdateDLLs(BuildTarget target)
        {
            CheckHotUpdateDllsPath();
            var projDir = HybridCLR.Editor.SettingsUtil.ProjectDir;
            var hotUpdateDllsPath = HybridCLR.Editor.SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            foreach (var hotUpdateDllName in HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyNamesIncludePreserved)
            {
                var srcPath = hotUpdateDllsPath + "/" + hotUpdateDllName + ".dll";
                var dstPath = _hotUpdateDllsPath + "/" + hotUpdateDllName + ".dll.bytes";
                srcPath = Path.Combine(projDir, srcPath);
                dstPath = Path.Combine(projDir, dstPath);
                Debug.Log($"Copy HotUpdate Dll {srcPath} to {dstPath}");
                File.Copy(srcPath, dstPath, true);
            }
        }
#endif
#endregion

    }
}