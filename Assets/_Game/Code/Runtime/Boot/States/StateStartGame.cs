using ProxFramework.StateMachine;

namespace Prox.GameName.Runtime
{
    public class StateStartGame : State
    {
        public override void Init()
        {
        }

        public override void Enter()
        {
            HotUpdateAssembly.CallStatic("GameName.Core.GameMain", "StartGame");
        }

        public override void Exit()
        {
        }
    }
}