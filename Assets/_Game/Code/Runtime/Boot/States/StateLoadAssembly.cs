using System;
using ProxFramework;
using ProxFramework.Runtime.Settings;
using ProxFramework.StateMachine;

namespace GameName.Runtime
{
    public class StateLoadAssembly : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            try
            {
                PLogger.Info("StateLoadAssembly");
#if !UNITY_EDITOR
                if (SettingsUtil.GlobalSettings.hclrSettings.Enable)
                {
                    GameAssembly.LoadMetadataForAOTAssembly().Forget();
                }

#endif
                if (SettingsUtil.GlobalSettings.hclrSettings.Enable)
                {
                    await GameAssembly.LoadHotUpdateAssemblies();
                    ChangeState<StateStartGame>();
                }
            }
            catch (Exception e)
            {
                PLogger.Exception(e.ToString());
            }
        }

        public override void Exit()
        {
        }
    }
}