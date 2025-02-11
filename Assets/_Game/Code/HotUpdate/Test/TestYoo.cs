using Cysharp.Threading.Tasks;
using Prox.GameName.UI;
using ProxFramework.UI;
using UnityEngine;

namespace Prox.GameName.HotFix
{
    public class TestYoo : MonoBehaviour
    {
        private void Awake()
        {
            UIModule.OpenWindowAsync<UITestAssetsWindow>("Assets/_Game/Art/UI/Prefabs/UITestAssetsWindow.prefab").Forget();
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}