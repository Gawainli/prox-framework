#if UNITY_EDITOR

using System;
using GameName.Core;
using ProxFramework;
using UnityEditor;

namespace GameName.Editor
{
    public class CheckRuntimeTables
    {
        [MenuItem("Tools/Check Runtime Tables")]
        public static async void CheckTables()
        {
            try
            {
                await TableManager.Initialize();
                PLogger.Info($"{TableManager.Tables.TbTextL10N.DataList.Count}");
                PLogger.Info("CheckRuntimeTables.CheckTables: Tables checked");
            }
            catch (Exception e)
            {
                PLogger.Exception($"CheckRuntimeTables.CheckTables: ", e);
            }
            // var str = "bbbb";
            // PLogger.Error($"AAAAAAAAAAAAA {new ArgumentException()}");
        }
    }
}
#endif