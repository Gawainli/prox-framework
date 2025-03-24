﻿using GameName.Base.PlaySys;
using GameName.Core;
using GameName.DataTable;
using ProxFramework.Localization;

namespace cfg
{
    public class I18NTable : II18NTable
    {
        public string GetValue(string languageCode, string key)
        {
            if (TryGetValue(languageCode, key, out var value))
            {
                return value;
            }

            return string.Empty;
        }

        public bool TryGetValue(string languageCode, string key, out string value)
        {
            var findKey = GameSysCore.GetSystem<DataTableSystem>().Tables.TbTextL10N.DataMap.TryGetValue(key, out var textL10N);
            if (findKey)
            {
                switch (languageCode)
                {
                    case "zh-hans":
                        value = textL10N.Hans;
                        break;
                    case "zh-hant":
                        value = textL10N.Hant;
                        break;
                    case "en":
                        value = textL10N.En;
                        break;
                    case "ja":
                        value = textL10N.Jp;
                        break;
                    case "ko":
                        value = textL10N.Ko;
                        break;
                    default:
                        value = textL10N.Hans;
                        break;
                }
            }
            else
            {
                value = key;
            }

            return findKey;
        }
    }
}