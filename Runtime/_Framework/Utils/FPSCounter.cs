using System;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace ProxFramework.Utils
{
    public class FPSCounter : MonoBehaviour
    {
        [Range(1, 5)] public float scale = 1;
        public Color textColor = Color.black;
        
        private StringBuilder _infoBuilder = new StringBuilder();
        private float _avgFps;
        private float _currentFps;
        private float _fpsCount;
        private float _updateInfoTime;
        private float _updateInfoInterval = 0.5f;
        private int _gcStartCount;

        private void Awake()
        {
            _gcStartCount = GC.CollectionCount(0);
            DontDestroyOnLoad(gameObject);
        }

        private void UpdateFps()
        {
            _updateInfoTime += Time.unscaledDeltaTime;
            _fpsCount++;
            if (_updateInfoTime >= _updateInfoInterval)
            {
                _avgFps = _fpsCount / _updateInfoTime;
                _fpsCount = 0;
                _updateInfoTime = 0;
            }

            _currentFps = 1 / Time.unscaledDeltaTime;
        }

        private void OnGUI()
        {
            UpdateFps();
            _infoBuilder.Clear();
            _infoBuilder.Append($"FPS Avg:{Mathf.Round(_avgFps)} Current:{Mathf.Round(_currentFps)}\n");
            
            //memory info
            _infoBuilder.Append($"Reserved: {Mathf.Round(Profiler.GetTotalReservedMemoryLong() / 1048576f)} MB\n");
            _infoBuilder.Append($"Allocated: {Mathf.Round(Profiler.GetTotalAllocatedMemoryLong() / 1048576f)} MB\n");
            _infoBuilder.Append($"Unused: {Mathf.Round(Profiler.GetTotalUnusedReservedMemoryLong() / 1048576f)} MB\n");
            _infoBuilder.Append($"Mono Heap: {Mathf.Round(Profiler.GetMonoHeapSizeLong() / 1048576f)} MB\n");
            _infoBuilder.Append($"Mono Used: {Mathf.Round(Profiler.GetMonoUsedSizeLong() / 1048576f)} MB\n");
            
            //graphics info
            //only in development build
            _infoBuilder.Append($"GFX: {Mathf.Round(Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576f)} MB\n");
            
            //system info
            _infoBuilder.Append($"System Total: {Mathf.Round(SystemInfo.systemMemorySize)} MB\n");
            
            //gc count
            var gcCount = GC.CollectionCount(0);
            _infoBuilder.Append($"GC0 Add: {gcCount - _gcStartCount} Total: {gcCount}\n");

            //gui
            var location = new Rect(5, 5, 200 * scale, 200 * scale);
            var bg = Texture2D.linearGrayTexture;
            GUI.DrawTexture(location, bg, ScaleMode.StretchToFill);
            GUI.color = textColor;
            var fontSize = 18 * scale;
            GUI.skin.label.fontSize = Mathf.FloorToInt(fontSize);
            GUI.Label(location, _infoBuilder.ToString());
        }
    }
}