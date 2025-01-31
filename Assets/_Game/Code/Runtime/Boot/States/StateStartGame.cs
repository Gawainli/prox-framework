using Cysharp.Threading.Tasks;
using ProxFramework.Scene;
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
            SceneModule.ChangeTopSceneAsync("Assets/_Game/Scenes/Main.unity").Forget();
        }

        public override void Exit()
        {
        }
    }
}