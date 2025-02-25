using System;
using cfg;
using ProxFramework;
using ProxFramework.Localization;
using UnityEngine.UI;
using ProxFramework.UI;
using TMPro;

namespace Prox.GameName.UI
{
    [UIWindowAttribute(0, false)]
    public partial class UITestLocalization : UIWindow
    {
        public override void OnCreate()
        {
            AutoBind();
        }

        public override void OnRefresh()
        {
            try
            {
                LocalizationModule.SetTable(new I18NTable());
                dropdownSelLang.options.Clear();
                foreach (var lang in LocalizationModule.SupportLanguages)
                {
                    dropdownSelLang.options.Add(new TMP_Dropdown.OptionData(lang.ToString()));
                }

                dropdownSelLang.value = 0;

                dropdownSelLang.onValueChanged.AddListener((int index) =>
                {
                    LocalizationModule.ChangeLanguage(LocalizationModule.SupportLanguages[index]);
                });
            }
            catch (Exception e)
            {
                PLogger.Error($"UITestLocalization.OnRefresh: {e}");
            }
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnDestroy()
        {
        }
    }
}