using UnityEngine.UI;
using ProxFramework.UI;
using TMPro;

namespace Prox.GameName.UI
{
    public partial class UITestAssetsWindow : UIWindow
    {
        //==自动化变量开始
        public RawImage @rawTest; 
        public Button @btnLoadTex; 
        public Button @btnReleaseTex; 

        private void AutoBind()
        {
            //==自动化路径开始
            @rawTest = Q<RawImage>("@Raw_Test"); 
            @btnLoadTex = Q<Button>("@Btn_LoadTex"); 
            @btnReleaseTex = Q<Button>("@Btn_ReleaseTex"); 

        }
    }
}