using ProxFramework.Asset;
using ProxFramework.Runtime.Settings;
using ProxFramework.StateMachine;

namespace Prox.GameName.Runtime
{
    public class StatePatchDone : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.ClearProgressBar();
#endif
            fsm.Blackboard.ClearAll();
            foreach (var pkg in AssetModule.GetAllPackages())
            {
                await AssetModule.ClearUnusedCacheFilesAsync(pkg.PackageName);
            }

            if (SettingsUtil.GlobalSettings.hclrSettings.Enable)
            {
                ChangeState<StateLoadAssembly>();
            }
            else
            {
                ChangeState<StateStartGame>();
            }
        }

        public override void Exit()
        {
        }
    }
}