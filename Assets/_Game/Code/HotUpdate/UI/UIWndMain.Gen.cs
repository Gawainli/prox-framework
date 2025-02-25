using UnityEngine.UI;
using ProxFramework.UI;
using TMPro;
using UnityEngine;

namespace Prox.GameName.UI
{
    public partial class UIWndMain : UIWindow
    {
        //==自动化变量开始
        public GameObject @objMainBtns; 
        public Button @btnGoTestAssets; 
        public Button @btnGoTestI18N; 
        public Button @btnBack2Main; 

        private void AutoBind()
        {
            //==自动化路径开始
            @objMainBtns = Q("@Obj_MainBtns").gameObject; 
            @btnGoTestAssets = Q<Button>("@Obj_MainBtns/Viewport/Content/@Btn_GoTestAssets"); 
            @btnGoTestI18N = Q<Button>("@Obj_MainBtns/Viewport/Content/@Btn_GoTestI18N"); 
            @btnBack2Main = Q<Button>("@Btn_Back2Main"); 

        }
    }
}