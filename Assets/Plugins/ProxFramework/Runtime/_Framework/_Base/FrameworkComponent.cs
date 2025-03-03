﻿using ProxFramework.Asset;
using ProxFramework.Event;
using ProxFramework.Localization;
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
            UIModule.Initialize(transform.Find("UIRootCanvas").gameObject);
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
            LocalizationModule.Shutdown();
            NetworkModule.Shutdown();
            AssetModule.Shutdown();
            StateMachineModule.Shutdown();
            EventModule.Shutdown();
        }
    }
}