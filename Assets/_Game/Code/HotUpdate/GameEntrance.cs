using System;
using cfg;
using Cysharp.Threading.Tasks;
using GameName.Base;
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
                //Initialize i18n
                await LocalizationModule.Initialize(new I18NTable());
                RegisterAllSystems();
                await LoadAllAsync();
                SystemHelper.StateSystem.ChangeState<StateMain>();
            }
            catch (Exception e)
            {
                PLogger.Exception($"Game Exception: {e.Message}", e);
            }
        }

        public static void RegisterAllSystems()
        {
            GameSysCore.Initialize();
            GameSysCore.RegisterSystem<DataTableSystem>();
            GameSysCore.RegisterSystem<GameStateSystem>();
            GameSysCore.RegisterSystem<GameSingletonSystem>();
        }

        public static async UniTask LoadAllAsync()
        {
            await GameSysCore.GetSystem<DataTableSystem>().LoadAllTables();
        }
    }
}