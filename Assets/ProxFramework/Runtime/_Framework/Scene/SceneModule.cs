using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
using ProxFramework.Module;
using UnityEngine.SceneManagement;

namespace ProxFramework.Scene
{
    public class SceneModule : IModule
    {
        private static readonly Stack<UnityEngine.SceneManagement.Scene> _sceneStack = new Stack<UnityEngine.SceneManagement.Scene>();
        
        public static async UniTask ChangeSceneAsync(string path)
        {
            await UnloadStackScene();
            var scene = await AssetModule.LoadSceneAsync(path, LoadSceneMode.Additive);
            _sceneStack.Push(scene);
        }
        
        public static async UniTask UnloadStackScene()
        {
            // await UniTask.SwitchToMainThread();
            if (_sceneStack.TryPop(out var curScene))
            {
                await UnloadSceneAsync(curScene);
            }
        }
        
        public static async UniTask UnloadSceneAsync(UnityEngine.SceneManagement.Scene scene)
        {
            // await UniTask.SwitchToMainThread();
            await SceneManager.UnloadSceneAsync(scene);
        }
            
        
        
        public void Initialize(object userData = null)
        {
            Initialized = true;
        }

        public void Tick(float deltaTime, float unscaledDeltaTime)
        {
        }

        public void Shutdown()
        {
        }

        public int Priority { get; set; }
        public bool Initialized { get; set; }
    }
}