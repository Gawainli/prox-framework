﻿using System.Collections.Generic;
using System;

namespace ProxFramework.StateMachine
{
    public class StateMachine
    {
        public bool isRunning = false;
        public string name;
        public object Owner => _owner;
        public bool ManuallyTick { get; private set; }

        private object _owner;
        private readonly Dictionary<Type, State> _states;
        private State _currentState;
        private State _lastState;

        public Blackboard.Blackboard Blackboard { get; }

        private StateMachine(object owner, bool manuallyTick, string name)
        {
            _owner = owner;
            this.name = name;
            _states = new Dictionary<Type, State>();
            Blackboard = new Blackboard.Blackboard($"{name}.Blackboard");
            ManuallyTick = manuallyTick;
        }

        public static StateMachine Create(object owner, bool manuallyTick = false, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = owner.GetType().Name + ".FSM";
            }

            var fsm = new StateMachine(owner, manuallyTick, name);
            StateMachineModule.RegisterStateMachine(fsm);
            return fsm;
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
    }
}