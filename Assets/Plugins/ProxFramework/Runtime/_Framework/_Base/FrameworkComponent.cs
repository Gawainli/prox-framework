using ProxFramework.Asset;
using ProxFramework.Event;
using ProxFramework.Network;
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
            NetworkModule.Initialize();
            UIModule.Initialize();
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            EventModule.Tick(dt);
            StateMachineModule.Tick(dt);
            UIModule.Tick(dt);

            NetworkModule.Tick(Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            UIModule.Shutdown();
            NetworkModule.Shutdown();
            AssetModule.Shutdown();
            StateMachineModule.Shutdown();
            EventModule.Shutdown();
        }
    }
}