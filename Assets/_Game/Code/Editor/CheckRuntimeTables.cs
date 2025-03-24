#if UNITY_EDITOR

using System;
using GameName.Base.PlaySys;
using GameName.Core;
using GameName.DataTable;
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
                await GameEntrance.InitSys();
                PLogger.Info($"{GameSysCore.GetSystem<DataTableSystem>().Tables.TbTextL10N.DataList.Count}");
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