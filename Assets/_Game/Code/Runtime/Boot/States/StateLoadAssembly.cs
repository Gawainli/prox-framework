using System;
using ProxFramework;
using ProxFramework.Runtime.Settings;
using ProxFramework.StateMachine;

namespace Prox.GameName.Runtime
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
                if (SettingsUtil.GlobalSettings.hclrSettings.Enable)
                {
#if !UNITY_EDITOR
                    await GameAssembly.LoadMetadataForAOTAssembly();
#endif
                    await GameAssembly.LoadHotUpdateAssemblies();
                    ChangeState<StateStartGame>();
                }
            }
            catch (Exception e)
            {
                PLogger.Error(e.ToString());
            }
        }

        public override void Exit()
        {
        }
    }
}