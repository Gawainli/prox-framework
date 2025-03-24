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
            HotUpdateAssembly.CallStatic("GameName.Core.GameEntrance", "Entrance");
        }

        public override void Exit()
        {
        }
    }
}