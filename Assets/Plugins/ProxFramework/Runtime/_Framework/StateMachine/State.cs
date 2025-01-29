namespace ProxFramework.StateMachine
{
    public abstract class State
    {
        public float age;
        public float fixedAge;

        protected StateMachine fsm;

        public State()
        {
        }

        //创建时调用
        public abstract void Init();

        //进入状态时调用
        public abstract void Enter();

        //退出状态时调用
        public abstract void Exit();

        public virtual void Tick(float delta)
        {
            age += delta;
        }

        public virtual void FixedTick(float delta)
        {
            fixedAge += delta;
        }

        public void SetParent(StateMachine p)
        {
            fsm = p;
        }

        public void ChangeState<T>() where T : State
        {
            if (fsm != null)
            {
                fsm.ChangeState<T>();
            }
        }
    }
}