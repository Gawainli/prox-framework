using UnityEngine;

namespace GameName.Base.PlaySys
{
    public class GameSysBehaviour : MonoBehaviour
    {
        private void FixedUpdate()
        {
            GameSysCore.FixedUpdate(Time.fixedTime);
        }

        private void Update()
        {
            GameSysCore.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
            GameSysCore.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
        
        private void OnApplicationQuit()
        {
            GameSysCore.OnApplicationQuit();
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            GameSysCore.OnApplicationPause(pauseStatus);
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            GameSysCore.OnApplicationFocus(hasFocus);
        }

        private void OnDrawGizmos()
        {
            GameSysCore.OnDrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            GameSysCore.OnDrawGizmosSelected();
        }
    }
}