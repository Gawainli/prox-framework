using ProxFramework;

namespace GameName.GamePlay
{
    public class StateMain : ProxFramework.StateMachine.State
    {
        public override void Init()
        {
        }

        public override void Enter()
        {
            PLogger.Info("StateMain Enter");
        }

        public override void Exit()
        {
        }
    }
}