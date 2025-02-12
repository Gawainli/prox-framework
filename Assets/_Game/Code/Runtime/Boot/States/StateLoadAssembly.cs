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
#if !UNITY_EDITOR
                await GameAssembly.LoadMetadataForAOTAssembly();
#endif
                await GameAssembly.LoadHotUpdateAssemblies();
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