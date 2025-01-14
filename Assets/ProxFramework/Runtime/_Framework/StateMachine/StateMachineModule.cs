using System.Collections.Generic;
using ProxFramework.Base;

using ProxFramework.Module;

namespace ProxFramework.StateMachine
{
    public class StateMachineModule : IModule
    {
        private static readonly Dictionary<string, List<StateMachine>> StateMachineDict = new Dictionary<string, List<StateMachine>>();
        
        public static StateMachine Create<T>(T owner, params State[] states) where T : class
        {
            if (owner == null)
            {
                return null;
            }

            var stateMachine = new StateMachine(owner);
            foreach (var state in states)
            {
                stateMachine.AddState(state);
            }
            
            RegisterStateMachine(stateMachine.name, stateMachine);
            return stateMachine;
        }
        
        public static List<StateMachine> GetStateMachine(string name)
        {
            if (StateMachineDict.TryGetValue(name, out var machines))
            {
                return machines;
            }
            return null;
        }
        
        public static void RegisterStateMachine(string name, StateMachine stateMachine)
        {
            if (StateMachineDict.TryGetValue(name, out var machines))
            {
                machines.Add(stateMachine);
                return;
            }
            StateMachineDict.Add(name, new List<StateMachine>(){stateMachine});
        }
        
        public static void UnRegisterStateMachine(StateMachine stateMachine)
        {
            stateMachine.ClearState();
            if (StateMachineDict.TryGetValue(stateMachine.name, out var machines))
            {
                machines.Remove(stateMachine);
            }
        }

        public void Initialize(object userData = null)
        {
            Logger.Info("StateMachineModule Initialize");
            Initialized = true;
        }

        public void Tick(float deltaTime, float unscaledDeltaTime)
        {
            foreach (var machines in StateMachineDict.Values)
            {
                foreach (var machine in machines)
                {
                   machine.Tick(deltaTime); 
                }
            }
        }

        public void Shutdown()
        {
            StateMachineDict.Clear();
        }

        public int Priority { get; set; }
        public bool Initialized { get; set; }
    }
}