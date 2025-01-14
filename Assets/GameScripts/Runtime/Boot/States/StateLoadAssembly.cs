using System.Reflection;
using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
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
            Logger.Info("StateLoadAssembly");
            var data = await AssetModule.LoadRawFileAsync("Assets/HotUpdateDll/ADF_Base.dll.bytes");
            var hotUpdate = Assembly.Load(data);
            var type = hotUpdate.GetType("HotUpdateLoader");
            var func = type.GetMethod("LoadMetadataForAOTAssemblies");
            if (func == null)
            {
                Logger.Error("func == null");
                return;
            }
            
            var result = (bool)func.Invoke(null, null);
            if (!result)
            {
                Logger.Error("LoadMetadataForAOTAssemblies failed");
            }
            
            func = type.GetMethod("LoadHotUpdateDlls");
            if (func == null)
            {
                Logger.Error("func == null");
                return;
            }
            result = (bool)func.Invoke(null, null);
            if ( result )
            {
                Logger.Info("LoadHotUpdateDlls success");
                ChangeState<StateStartGame>();
            }
            else
            {
                Logger.Error("LoadHotUpdateDlls failed");
            }
        }

        public override void Exit()
        {
        }
    }
}