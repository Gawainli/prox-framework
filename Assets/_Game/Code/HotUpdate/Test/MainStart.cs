using System;
using Cysharp.Threading.Tasks;
using Prox.GameName.UI;
using ProxFramework.UI;
using UnityEngine;

namespace Prox.GameName.HotFix
{
    public class MainStart : MonoBehaviour
    {
        private void Start()
        {
            UIModule.OpenWindowAsync<UIWndMain>("Assets/_Game/Art/UI/Prefabs/UIWndMain.prefab").Forget();
        }
    }
}