using System.Reflection;
using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
using ProxFramework.Logger;
using ProxFramework.Scene;
using ProxFramework.StateMachine;

namespace ProxFramework.Base
{
    public class StateLoadAssembly : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            LogModule.Info("StateLoadAssembly");
            var data = await AssetModule.LoadRawFileAsync("Assets/HotUpdateDll/ADF_Base.dll.bytes");
            var hotUpdate = Assembly.Load(data);
            var type = hotUpdate.GetType("HotUpdateLoader");
            var func = type.GetMethod("LoadMetadataForAOTAssemblies");
            if (func == null)
            {
                LogModule.Error("func == null");
                return;
            }
            
            var result = (bool)func.Invoke(null, null);
            if (!result)
            {
                LogModule.Error("LoadMetadataForAOTAssemblies failed");
            }
            
            func = type.GetMethod("LoadHotUpdateDlls");
            if (func == null)
            {
                LogModule.Error("func == null");
                return;
            }
            result = (bool)func.Invoke(null, null);
            if ( result )
            {
                LogModule.Info("LoadHotUpdateDlls success");
                ChangeState<StateStartGame>();
            }
            else
            {
                LogModule.Error("LoadHotUpdateDlls failed");
            }
        }

        public override void Exit()
        {
        }
    }
}