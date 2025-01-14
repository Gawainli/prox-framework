
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProxFramework.UI
{
    public abstract class UIWidget
    {
        protected readonly RectTransform rootTransform;

        protected UIWidget(RectTransform widgetRoot)
        {
            rootTransform = widgetRoot;
        }
        
        public abstract void Create();

        public virtual void Show()
        {
            rootTransform.gameObject.SetActive(true);
        }
        
        public virtual void Hide()
        {
            rootTransform.gameObject.SetActive(false);
        }
        
        protected T Q<T>(string path) where T : Component
        {
            var transform = rootTransform.Find(path);
            if (transform == null)
            {
                Logger.Error($"Widget:{GetType().Name} Q Error: {path} is null");
            }
            return transform.GetComponent<T>();
        }
    }
}