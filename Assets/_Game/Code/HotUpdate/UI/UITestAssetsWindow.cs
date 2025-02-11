using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
using ProxFramework.UI;
using UnityEngine;

namespace Prox.GameName.UI
{
    [UIWindowAttribute(0, false)]
    public partial class UITestAssetsWindow : UIWindow
    {
        public override void OnCreate()
        {
            AutoBind();
        }

        public override void OnRefresh()
        {
            btnLoadTex.onClick.AddListener(LoadTexture);
            btnReleaseTex.onClick.AddListener(ReleaseTexture);
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnDestroy()
        {
            btnLoadTex.onClick.RemoveListener(LoadTexture);
            btnReleaseTex.onClick.RemoveListener(ReleaseTexture);
        }
        
        private void LoadTexture()
        {
            AssetModule.LoadAssetAsync<Texture2D>("Assets/_Game/Art/UI/test.jpg").ContinueWith(tex =>
            {
                rawTest.texture = tex;
            }).Forget();
        }
        
        private void ReleaseTexture()
        {
            var tex = rawTest.texture;
            rawTest.texture = null;
            // AssetModule.ReleaseAsset("Assets/_Game/Art/UI/test.jpg");
            AssetModule.ReleaseAsset(tex);
            AssetModule.UnloadUnusedAssets().Forget();
        }
    }
}