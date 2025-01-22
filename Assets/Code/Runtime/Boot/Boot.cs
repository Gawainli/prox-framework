using UnityEngine;
using ProxFramework.StateMachine;

namespace GameName.Runtime
{
    public class Boot : MonoBehaviour
    {
        private StateMachine _bootStateMachine;

        private void Start()
        {
            InitBootState();
            _bootStateMachine.Start<StateLaunch>();
        }

        private void Update()
        {
        }

        private void InitBootState()
        {
            _bootStateMachine = StateMachine.Create(this);
            _bootStateMachine.AddState<StateLaunch>();
            _bootStateMachine.AddState<StateSplash>();
            _bootStateMachine.AddState<StateInitPackage>();
            _bootStateMachine.AddState<StateUpdateVersion>();
            _bootStateMachine.AddState<StateUpdateManifest>();
            _bootStateMachine.AddState<StateCreateDownloader>();
            _bootStateMachine.AddState<StateDownloadFile>();
            _bootStateMachine.AddState<StatePatchDone>();
            _bootStateMachine.AddState<StateLoadAssembly>();
            _bootStateMachine.AddState<StateStartGame>();
        }
    }
}