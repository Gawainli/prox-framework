using System.Collections.Generic;
using System;

namespace ProxFramework.StateMachine
{
    public class StateMachine
    {
        public bool isRunning = false;
        public string name;
        public object Owner => _owner;

        private object _owner;
        private readonly Dictionary<Type, State> _states;
        private State _currentState;
        private State _lastState;
        private Dictionary<string, object> _blackboard = new();

        public StateMachine(object owner)
        {
            _owner = owner;
            name = owner.GetType().Name + ".FSM";
            _states = new Dictionary<Type, State>();
        }

        ~StateMachine()
        {
            _owner = null;
            _states.Clear();
        }

        public void Start<T>() where T : State
        {
            _currentState = GetState(typeof(T));
            _lastState = _currentState;
            if (_currentState == null)
            {
                PLogger.Error("Start StateMachine Failed, State is Null");
                return;
            }

            isRunning = true;
            _currentState.SetParent(this);
            _currentState.Enter();
        }

        public State GetState(Type stateType)
        {
            if (stateType == null)
            {
                return null;
            }

            if (_states.TryGetValue(stateType, out var state))
            {
                return state;
            }

            return null;
        }

        public State GetCurrentState()
        {
            return _currentState;
        }

        public void AddState<T>() where T : State
        {
            var type = typeof(T);
            var state = Activator.CreateInstance<T>();
            if (state == null)
            {
                return;
            }

            AddState(state);
        }


        public void AddState(State state)
        {
            if (state == null)
            {
                return;
            }

            if (_states.ContainsKey(state.GetType()))
            {
                return;
            }

            state.SetParent(this);
            _states.Add(state.GetType(), state);
            state.Init();
        }

        public void Tick(float delta)
        {
            if (!isRunning)
            {
                return;
            }

            if (_currentState != null)
            {
                _currentState.Tick(delta);
            }
        }

        public void FixedTick(float delta)
        {
            if (!isRunning)
            {
                return;
            }

            if (_currentState != null)
            {
                _currentState.FixedTick(delta);
            }
        }

        public void ChangeState<T>() where T : State
        {
            if (!isRunning)
            {
                PLogger.Error($"{name} ChangeState Failed, StateMachine is not Running");
                return;
            }

            var state = GetState(typeof(T));
            if (state == null)
            {
                PLogger.Error($"{typeof(T)} ChangeState Failed, State is Null");
                return;
            }

            if (_currentState != null)
            {
                _currentState.Exit();
            }

            _lastState = _currentState;
            _currentState = state;
            _currentState.SetParent(this);
            _currentState.Enter();
        }

        public string GetStateName()
        {
            return _currentState.GetType().Name;
        }

        public string GetLastStateName()
        {
            return _lastState.GetType().Name;
        }

        public bool IsCurrentState<T>() where T : State
        {
            return _currentState.GetType() == typeof(T);
        }

        public bool IsLastState<T>() where T : State
        {
            return _lastState.GetType() == typeof(T);
        }

        public void ClearState()
        {
            _currentState.Exit();
            _currentState = null;
        }

        public string GetStateInfo()
        {
            return $"{name} CurrentState: {_currentState.GetType().Name} LastState: {_lastState.GetType().Name}";
        }

        public void SetBlackboardValue(string key, object value)
        {
            if (!_blackboard.TryAdd(key, value))
            {
                _blackboard[key] = value;
            }
        }

        public void SetBlackboardValue<T>(string key, T value)
        {
            if (!_blackboard.TryAdd(key, value))
            {
                _blackboard[key] = value;
            }
        }

        public object GetBlackboardValue(string key)
        {
            return _blackboard.GetValueOrDefault(key);
        }

        public T GetBlackboardValue<T>(string key)
        {
            return (T)_blackboard.GetValueOrDefault(key);
        }
    }
}