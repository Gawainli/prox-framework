using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
using UnityEngine.SceneManagement;

namespace ProxFramework.Scene
{
    public class SceneModule
    {
        private static Stack<UnityEngine.SceneManagement.Scene> _sceneStack = new();

        public static async UniTask PushSceneAsync(string location)
        {
            var scene = await AssetModule.LoadSceneAsync(location, LoadSceneMode.Additive);
            _sceneStack.Push(scene);
        }

        public static async UniTask ChangeTopSceneAsync(string location)
        {
            await PopSceneAsync();
            await PushSceneAsync(location);
        }

        public static async UniTask PopSceneAsync()
        {
            if (_sceneStack.TryPop(out var curScene))
            {
                await UnloadSceneAsync(curScene);
            }
        }

        private static async UniTask UnloadSceneAsync(UnityEngine.SceneManagement.Scene scene)
        {
            await AssetModule.UnloadSceneAsync(scene);
        }
    }
}