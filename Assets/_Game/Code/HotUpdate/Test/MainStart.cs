using Cysharp.Threading.Tasks;
using Luban;
using Prox.GameName.UI;
using ProxFramework;
using ProxFramework.Asset;
using ProxFramework.UI;
using UnityEngine;

namespace Prox.GameName.Test
{
    public class MainStart : MonoBehaviour
    {
        private void Start()
        {
            var t = new TestClass();
            t.TestMethod();

            PLogger.Info("start game. open main window");
            // UIModule.OpenWindowAsync<UIWndMain>("Assets/_Game/Art/UI/Prefabs/UIWndMain.prefab").Forget();
        }
    }
}