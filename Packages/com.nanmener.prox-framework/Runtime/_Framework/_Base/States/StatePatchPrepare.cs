using ProxFramework.StateMachine;

namespace ProxFramework.Base
{
    public class StatePatchPrepare : State
    {
        public override void Init()
        {
            
        }

        public override void Enter()
        {
            ChangeState<StateInitPackage>();
        }

        public override void Exit()
        {
        }
    }
}