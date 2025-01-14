using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProxFramework.Module
{
    public class ModuleCore
    {
        private static readonly Dictionary<Type, IModule> ModulesDict = new Dictionary<Type, IModule>();

        public static T CreateModule<T>(object userData = null, int priority = 0) where T : class, IModule, new()
        {
            var type = typeof(T);
            if (ModulesDict.ContainsKey(type))
            {
                Logger.Error($"Module is already created. {type.FullName}");
                return null;
            }

            var module = new T
            {
                Priority = priority
            };
            ModulesDict.Add(type, module);
            module.Initialize(userData);
            return module;
        }

        public static bool ContainsModule<T>() where T : class, IModule
        {
            var type = typeof(T);
            return ModulesDict.ContainsKey(type);
        }

        public static T GetModule<T>() where T : class, IModule
        {
            var type = typeof(T);
            if (ModulesDict.TryGetValue(type, out var value))
            {
                return value as T;
            }

            return null;
        }

        public static void RemoveModule<T>() where T : class, IModule
        {
            var type = typeof(T);
            if (ModulesDict.ContainsKey(type))
            {
                ModulesDict.Remove(type);
            }
        }

        public static void TickAllModules(float deltaTime, float unscaledDeltaTime)
        {
            foreach (var module in ModulesDict.Values)
            {
                if (!module.Initialized)
                {
                    continue;
                }
                module.Tick(deltaTime, unscaledDeltaTime);
            }
        }

        public static void ShutdownAllModules()
        {
            foreach (var module in ModulesDict.Values)
            {
                module.Shutdown();
            }
        }
    }
}