using System.Reflection;
using ProxFramework;
using ProxFramework.Asset;
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
            PLogger.Info("StateLoadAssembly");
            var data = await AssetModule.LoadRawDataAsync("Assets/HotUpdateDll/ADF_Base.dll.bytes");
            var hotUpdate = Assembly.Load(data);
            var type = hotUpdate.GetType("HotUpdateLoader");
            var func = type.GetMethod("LoadMetadataForAOTAssemblies");
            if (func == null)
            {
                PLogger.Error("func == null");
                return;
            }
            
            var result = (bool)func.Invoke(null, null);
            if (!result)
            {
                PLogger.Error("LoadMetadataForAOTAssemblies failed");
            }
            
            func = type.GetMethod("LoadHotUpdateDlls");
            if (func == null)
            {
                PLogger.Error("func == null");
                return;
            }
            result = (bool)func.Invoke(null, null);
            if ( result )
            {
                PLogger.Info("LoadHotUpdateDlls success");
                ChangeState<StateStartGame>();
            }
            else
            {
                PLogger.Error("LoadHotUpdateDlls failed");
            }
        }

        public override void Exit()
        {
        }
    }
}