using ProxFramework.StateMachine;

namespace GameName.Runtime
{
    public class StateLaunch : State
    {
        public override void Init()
        {
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Tick(float delta)
        {
            ChangeState<StateSplash>();
        }
    }
}