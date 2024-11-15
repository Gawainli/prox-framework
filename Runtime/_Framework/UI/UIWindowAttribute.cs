using System;

namespace ProxFramework.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIWindowAttribute : Attribute
    {
        //layer: 高的在前
        public int windowLayer;
        public bool fullScreen;

        public UIWindowAttribute(int layer, bool full)
        {
            windowLayer = layer;
            fullScreen = full;
        }
    }
}