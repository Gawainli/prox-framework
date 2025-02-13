using System;
using ProxFramework;
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
#if ENABLE_HCLR
#if UNITY_EDITOR
                PLogger.Warning("Skip loading, and only verify the downloaded assemblies in Editor.");
                await HotUpdateAssembly.CheckAllAssemblies();
#else
                await HotUpdateAssembly.LoadHotUpdateAssemblies();
                await HotUpdateAssembly.LoadMetadataForAOTAssembly();
#endif
#endif
                ChangeState<StateStartGame>();
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