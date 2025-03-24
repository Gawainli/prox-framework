using GameName.Base.PlaySys;
using ProxFramework.StateMachine;

namespace GameName.GamePlay
{
    public class GamePlaySys : AbsGameSys
    {
        private StateMachine _gameplayFsm;

        public override void Init()
        {
            _gameplayFsm = StateMachine.Create(this, true);
            _gameplayFsm.AddState<StateMain>();
        }

        public override void Shutdown()
        {
        }

        public override void Update(float deltaTime, float unscaledDeltaTime)
        {
            base.Update(deltaTime, unscaledDeltaTime);
            if (_gameplayFsm is { isRunning: true })
            {
                _gameplayFsm.Tick(deltaTime);
            }
        }

        public void StartGame()
        {
            _gameplayFsm.Start<StateMain>();
        }
    }
}