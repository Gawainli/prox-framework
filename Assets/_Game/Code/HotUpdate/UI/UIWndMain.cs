using Cysharp.Threading.Tasks;
using ProxFramework.UI;

namespace Prox.GameName.UI
{
    [UIWindowAttribute(100, false)]
    public partial class UIWndMain : UIWindow
    {
        public override void OnCreate()
        {
            AutoBind();
        }

        public override void OnRefresh()
        {
            btnGoTestAssets.onClick.AddListener(() =>
            {
                objMainBtns.SetActive(false);
                UIModule.OpenWindowAsync<UITestAssetsWindow>("Assets/_Game/Art/UI/Prefabs/UITestAssetsWindow.prefab")
                    .Forget();
                btnBack2Main.onClick.RemoveAllListeners();
                btnBack2Main.onClick.AddListener(() =>
                {
                    objMainBtns.SetActive(true);
                    UIModule.CloseWindow<UITestAssetsWindow>();
                    btnBack2Main.onClick.RemoveAllListeners();
                });
            });
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnDestroy()
        {
            btnBack2Main.onClick.RemoveAllListeners();
            btnGoTestAssets.onClick.RemoveAllListeners();
        }
    }
}