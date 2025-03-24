namespace GameName.Base.PlaySys
{
    public abstract class AbsGameSys
    {
        public abstract void Init();
        public abstract void Shutdown(); 
        
        public virtual void Update(float deltaTime, float unscaledDeltaTime)
        {
        }

        public virtual void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
        }

        public virtual void FixedUpdate(float fixedDeltaTime)
        {
        }
        
        public virtual void OnApplicationPause(bool pause)
        {
        }
        
        public virtual void OnApplicationFocus(bool focus)
        {
        }
        
        public virtual void OnApplicationQuit()
        {
        }
        
        public virtual void OnDrawGizmos()
        {
        }
        
        public virtual void OnDrawGizmosSelected()
        {
        }
    }
}