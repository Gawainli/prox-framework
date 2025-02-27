using System;
using UnityEngine;

namespace ProxFramework.Localization
{
    public abstract class LocalizedBehaviour : MonoBehaviour
    {
        [SerializeField] private bool enableLocalization = true;
        [SerializeField] protected string l10NKey;

        public virtual void AutoSetL10NKey()
        {
            l10NKey = $"{transform.parent.name}_{transform.name}";
        }
        
        public virtual void Init(){}

        public abstract void ApplyLocalization();

        protected virtual void Awake()
        {
        }

        protected void OnEnable()
        {
            if (enableLocalization && LocalizationModule.initialized)
            {
                LocalizationModule.OnLanguageChanged += ApplyLocalization;
                ApplyLocalization();
            }
        }

        protected void OnDisable()
        {
            if (enableLocalization && LocalizationModule.initialized)
            {
                LocalizationModule.OnLanguageChanged -= ApplyLocalization;
            }
        }
    }
}