using System;

namespace ProxFramework.UI
{
    [Serializable]
    public class UISettings
    {
        public bool pixelPerfect = false;
        public bool matchWidthOrHeight = false;
        public float referenceResolutionX = 1920;
        public float referenceResolutionY = 1080;
        public bool ignoreReversedGraphics = false; 
    }
}