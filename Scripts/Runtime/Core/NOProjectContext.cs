using Sirenix.Utilities;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NOProjectContext : NOScriptableObject
    {
        private const string ResourcesCorePath = "Core";
        public static NOProjectContext ProjectContext { get; private set; }

        [field: SerializeField]
        public NOManagerScriptableObject[] ScriptableObjectManagers { get; private set; }

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
                ScriptableObjectManagers.ForEach(m => m.Initialize());
            }
        }
        private void DisposeProjectContext()
        {
            Application.quitting -= DisposeProjectContext;
            if (!ScriptableObjectManagers.IsNullOrEmpty())
            {
                ScriptableObjectManagers.ForEach(m => m.Dispose());
            }
            ProjectContext = null;
        }
    }
}