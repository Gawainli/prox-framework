using System;
using cfg;
using Cysharp.Threading.Tasks;
using GameName.Base.PlaySys;
using GameName.DataTable;
using GameName.GamePlay;
using ProxFramework;
using ProxFramework.Localization;

namespace GameName.Core
{
    public static class GameEntrance
    {
        public static async void Entrance()
        {
            try
            {
                await InitSys();
            }
            catch (Exception e)
            {
                PLogger.Error(e.ToString());
            }
        }

        public static async UniTask InitSys()
        {
            //Initialize i18n
            await LocalizationModule.Initialize(new I18NTable());

            //Initialize GameSysCore
            GameSysCore.Initialize();
            GameSysCore.RegisterSystem(new DataTableSystem());
            await GameSysCore.GetSystem<DataTableSystem>().LoadAllTables();
            GameSysCore.RegisterSystem(new GamePlaySys());
        }
    }
}