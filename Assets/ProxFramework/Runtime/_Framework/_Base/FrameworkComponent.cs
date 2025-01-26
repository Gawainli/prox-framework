using ProxFramework.Asset;
using ProxFramework.Event;
using ProxFramework.StateMachine;
using ProxFramework.UI;
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
            UIModule.Initialize();
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            EventModule.Tick(dt);
            StateMachineModule.Tick(dt);
            UIModule.Tick(dt);
        }

        private void OnDestroy()
        {
            UIModule.Shutdown();
            AssetModule.Shutdown();
            StateMachineModule.Shutdown();
            
            EventModule.Shutdown();
        }
    }
}