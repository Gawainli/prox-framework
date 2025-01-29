using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProxFramework.UI
{
    public class ButtonEx : Button
    {
        private const float ClickInterval = 0.5f;
        private float _lastClickTime;

        [SerializeField] private GameObject[] normalStateObjects;
        [SerializeField] private GameObject[] highlightedStateObjects;
        [SerializeField] private GameObject[] pressedStateObjects;
        [SerializeField] private GameObject[] disabledStateObjects;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            UpdateChildObjects(state);
        }

        private void UpdateChildObjects(SelectionState state)
        {
            SetActiveState(normalStateObjects, state == SelectionState.Normal);
            SetActiveState(highlightedStateObjects, state == SelectionState.Highlighted);
            SetActiveState(pressedStateObjects, state == SelectionState.Pressed);
            SetActiveState(disabledStateObjects, state == SelectionState.Disabled);
        }

        private void SetActiveState(GameObject[] objects, bool isActive)
        {
            if (objects != null)
            {
                foreach (var obj in objects)
                {
                    if (obj != null)
                    {
                        obj.SetActive(isActive);
                    }
                }
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (Time.time - _lastClickTime < ClickInterval)
            {
                return;
            }

            _lastClickTime = Time.time;
            base.OnPointerClick(eventData);
        }

#if UNITY_EDITOR

        #region CONVERT

        [MenuItem("CONTEXT/Button/Convert To ButtonEx", true)]
        static bool _ConvertToButtonEx(MenuCommand command)
        {
            return CanConvertTo<ButtonEx>(command.context);
        }

        [MenuItem("CONTEXT/Button/Convert To ButtonEx", false)]
        static void ConvertToButtonEx(MenuCommand command)
        {
            ConvertTo<ButtonEx>(command.context);
        }

        [MenuItem("CONTEXT/Button/Convert To Button", true)]
        static bool _ConvertToButton(MenuCommand command)
        {
            return CanConvertTo<Button>(command.context);
        }

        [MenuItem("CONTEXT/Button/Convert To Button", false)]
        static void ConvertToButton(MenuCommand command)
        {
            ConvertTo<Button>(command.context);
        }

        /// <summary>
        /// Verify whether it can be converted to the specified component.
        /// </summary>
        protected static bool CanConvertTo<T>(Object context)
            where T : MonoBehaviour
        {
            return context && context.GetType() != typeof(T);
        }

        /// <summary>
        /// Convert to the specified component.
        /// </summary>
        protected static void ConvertTo<T>(Object context) where T : MonoBehaviour
        {
            var target = context as MonoBehaviour;
            var so = new SerializedObject(target);
            so.Update();

            bool oldEnable = target != null && target.enabled;
            if (target != null) target.enabled = false;

            // Find MonoScript of the specified component.
            foreach (var script in Resources.FindObjectsOfTypeAll<MonoScript>())
            {
                if (script.GetClass() != typeof(T))
                    continue;

                // Set 'm_Script' to convert.
                so.FindProperty("m_Script").objectReferenceValue = script;
                so.ApplyModifiedProperties();
                break;
            }

            ((MonoBehaviour)so.targetObject).enabled = oldEnable;
        }

        #endregion

#endif
    }
}