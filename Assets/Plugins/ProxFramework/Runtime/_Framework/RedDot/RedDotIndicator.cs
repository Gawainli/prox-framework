using UnityEngine;

namespace ProxFramework.RedDot
{
    public abstract class RedDotIndicator : MonoBehaviour
    {
        [SerializeField] private string redDotId;

        private RedDotNode _rdNode;
        protected abstract void OnRedDotNodeChanged(RedDotNode redDotNode);

        protected void Awake()
        {
            _rdNode = RedDotModule.GetOrCreateNode(redDotId);
            _rdNode.OnStateChanged += OnRedDotNodeChanged;
        }

        protected void OnDestroy()
        {
            if (_rdNode != null)
            {
                _rdNode.OnStateChanged -= OnRedDotNodeChanged;
            }
        }
    }
}