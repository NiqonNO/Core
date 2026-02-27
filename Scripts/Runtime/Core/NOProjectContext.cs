using Sirenix.Utilities;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NOProjectContext : NOScriptableObject
    {
        private const string ResourcesCorePath = "Core";
        private static NOProjectContext ProjectContext { get; set; }

        [field: SerializeField]
        public NOManagerSO[] ScriptableObjectManagers { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            var projectContexts = Resources.LoadAll<NOProjectContext>(ResourcesCorePath);
            if (projectContexts.IsNullOrEmpty())
            {
                Debug.LogError($"Could not find object of type {nameof(NOProjectContext)} in Resources \"{ResourcesCorePath}\" folder. Project will not be initialized.");
                return;
            }
            if (projectContexts.Length > 1)
            {
                Debug.LogWarning($"More than one objects of type {nameof(NOProjectContext)} have been found in Resources \"{ResourcesCorePath}\" folder. First result will be used.");
            }

            ProjectContext = projectContexts[0];
            ProjectContext.SetupProjectContext();
        }

        private void SetupProjectContext()
        {
            Application.quitting += DisposeProjectContext;

            if (!ScriptableObjectManagers.IsNullOrEmpty())
            {
                RegisterServices();
                InitializeServices();
            }
        }

        private void DisposeProjectContext()
        {
            Application.quitting -= DisposeProjectContext;

            if (!ScriptableObjectManagers.IsNullOrEmpty())
            {
                DisposeServices();
            }

            ProjectContext = null;
        }
        
        private void RegisterServices()
        {
            foreach (var manager in ScriptableObjectManagers)
            {
                if (CheckNull(manager)) continue;
                NOContainer.RegisterService(manager);
            }
        }

        private void InitializeServices()
        {
            foreach (var manager in ScriptableObjectManagers)
            {
                if (CheckNull(manager)) continue;
                manager.Initialize();
            }
        }
        
        private void DisposeServices()
        {
            foreach (var manager in ScriptableObjectManagers)
            {
                if (CheckNull(manager)) continue;
                NOContainer.UnregisterService(manager);
                manager.Dispose();
            }
        }
        
        bool CheckNull(INOManager manager)
        {
            if (manager != null) return false;
            Debug.LogWarning($"Null manager in ScriptableObjectManagers array in Project context", this);
            return true;
        }
    }
}