using System.Collections.Generic;

namespace ProxFramework.StateMachine
{
    public static class StateMachineModule
    {
        private static bool _initialized = false;

        private static readonly Dictionary<string, List<StateMachine>> StateMachineDict =
            new Dictionary<string, List<StateMachine>>();

        public static List<StateMachine> GetStateMachine(string name)
        {
            return StateMachineDict.GetValueOrDefault(name);
        }

        public static void RegisterStateMachine(StateMachine stateMachine)
        {
            if (!StateMachineDict.TryGetValue(stateMachine.name, out var stateMachineList))
            {
                stateMachineList = new List<StateMachine>();
                StateMachineDict[stateMachine.name] = stateMachineList;
            }

            stateMachineList.Add(stateMachine);
        }

        public static void UnRegisterStateMachine(StateMachine stateMachine)
        {
            stateMachine.ClearState();
            if (StateMachineDict.TryGetValue(stateMachine.name, out var machines))
            {
                machines.Remove(stateMachine);
            }
        }

        public static void Initialize()
        {
            PLogger.Info("StateMachineModule Initialize");
            _initialized = true;
        }

        public static void Tick(float deltaTime)
        {
            if (!_initialized) return;
            foreach (var machines in StateMachineDict.Values)
            {
                foreach (var machine in machines)
                {
                    machine.Tick(deltaTime);
                }
            }
        }

        public static void Shutdown()
        {
            StateMachineDict.Clear();
        }
    }
}