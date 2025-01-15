using ProxFramework.Asset;
using ProxFramework.StateMachine;
using UnityEngine;

namespace GameName.Runtime
{
    public class StateStartGame : State
    {
        public override void Init()
        {
        }

        public override void Enter()
        {
            //todo: game start logic. enter first game scene

            var sp = AssetModule.LoadAssetSync<Sprite>("Assets/AssetsArt/UI/test.jpg");
            Debug.Log("Loaded sprite: " + sp.name);
        }

        public override void Exit()
        {
        }
    }
}