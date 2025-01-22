using System;
using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
using ProxFramework.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameName.Runtime
{
    public class StateStartGame : State
    {
        public override void Init()
        {
        }

        public override void Enter()
        {
            AssetModule.LoadSceneAsync("Assets/_Scenes/TestYoo.unity", LoadSceneMode.Additive).Forget();
        }

        public override void Exit()
        {
        }
    }
}