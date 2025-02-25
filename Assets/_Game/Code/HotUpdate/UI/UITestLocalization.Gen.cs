using UnityEngine.UI;
using ProxFramework.UI;
using TMPro;
using UnityEngine;

namespace Prox.GameName.UI
{
    public partial class UITestLocalization : UIWindow
    {
        //==自动化变量开始
        public TMP_Dropdown @dropdownSelLang; 

        private void AutoBind()
        {
            //==自动化路径开始
            @dropdownSelLang = Q<TMP_Dropdown>("@Dropdown_SelLang"); 

        }
    }
}