using ProxFramework.Asset;
using ProxFramework.Event;
using ProxFramework.StateMachine;
using UnityEngine;

namespace ProxFramework.Base
{
    public class FrameworkComponent : MonoBehaviour
    {
        private void Start()
        {
            EventModule.Initialize();
            StateMachineModule.Initialize();
            AssetModule.Initialize();
        }

        private void Update()
        {
            EventModule.Tick(Time.deltaTime);
            StateMachineModule.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            AssetModule.Shutdown();
            EventModule.Shutdown();
            StateMachineModule.Shutdown();
        }
    }
}