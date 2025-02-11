using ProxFramework.StateMachine;

namespace GameName.Runtime
{
    public class StateStartGame : State
    {
        public override void Init()
        {
        }

        public override void Enter()
        {
            GameAssembly.CallStatic("GameName.GameMain.GameMain", "StartGame");
        }

        public override void Exit()
        {
        }
    }
}