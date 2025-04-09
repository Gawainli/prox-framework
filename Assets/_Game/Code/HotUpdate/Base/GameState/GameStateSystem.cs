using GameName.Base.PlaySys;
using ProxFramework.StateMachine;

namespace GameName.GamePlay
{
    public class GameStateSystem : AbsGameSys
    {
        private StateMachine _gameplayFsm;
        private bool _isInit;

        public override void Init()
        {
            if (_isInit)
            {
                return;
            }
            
            _gameplayFsm = StateMachine.Create(this, true);
            _gameplayFsm.AddState<StateMain>();
        }

        public override void Shutdown()
        {
            if (!_isInit)
            {
                return;
            }
            
            if (_gameplayFsm != null)
            {
                StateMachineModule.UnRegisterStateMachine(_gameplayFsm);
                _gameplayFsm = null;
            }

            _isInit = false;
        }

        public override void Update(float deltaTime, float unscaledDeltaTime)
        {
            if (!_isInit)
            {
                return;
            }
            
            base.Update(deltaTime, unscaledDeltaTime);
            if (_gameplayFsm is { isRunning: true })
            {
                _gameplayFsm.Tick(deltaTime);
            }
        }

        public void ChangeState<T>() where T : State
        {
            _gameplayFsm.ChangeState<T>();
        }
    }
}