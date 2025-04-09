using GameName.Base;
using GameName.Base.PlaySys;
using GameName.DataTable;

namespace GameName.GamePlay
{
    public static class SystemHelper
    {
        public static GameSingletonSystem SingletonSystem => GameSysCore.GetSystem<GameSingletonSystem>();
        public static GameStateSystem StateSystem => GameSysCore.GetSystem<GameStateSystem>();
        public static cfg.Tables Tables => GameSysCore.GetSystem<DataTableSystem>().Tables;
    }
}