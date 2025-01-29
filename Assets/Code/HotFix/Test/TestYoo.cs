using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
using UnityEngine;
using UnityEngine.UI;

namespace Prox.GameName.HotFix
{
    public class TestYoo : MonoBehaviour
    {
        public RawImage rawImage;
        public Button button;

        private void Awake()
        {
            button.onClick.AddListener(ReleaseTexture);
        }

        // Start is called before the first frame update
        private async void Start()
        {
            var tex = await AssetModule.LoadAssetAsync<Texture2D>("Assets/AssetsArt/UI/test.jpg");

            Debug.Log(tex.GetHashCode());
            await AssetModule.UnloadUnusedAssets();
            // var tex2 = await AssetModule.LoadAssetAsync<Texture2D>("Assets/AssetsArt/UI/test.jpg");
            var tex2 = UnityEngine.Object.Instantiate(tex);
            Debug.Log(tex2.GetHashCode());

            rawImage.texture = tex2;
            rawImage.SetNativeSize();
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void ReleaseTexture()
        {
            var tex = rawImage.texture;
            rawImage.texture = null;
            AssetModule.ReleaseAsset("Assets/AssetsArt/UI/test.jpg");
            // AssetModule.ReleaseAsset(tex);
            AssetModule.UnloadUnusedAssets().Forget();
        }
    }
}