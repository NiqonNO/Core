using Sirenix.Utilities;
using UnityEngine;

namespace NiqonNO.Core
{
    public static class NOProjectManager
    {
        const string ResourcesCorePath = "Core";
        public static NOProjectContext ProjectContext { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            Debug.Log("Project Initialize");
            var projectContexts = Resources.LoadAll<NOProjectContext>(ResourcesCorePath);
            if(projectContexts.IsNullOrEmpty())
            {
                Debug.LogError($"Could not find object of type {nameof(NOProjectContext)} in Resources \"{ResourcesCorePath}\" folder. Project will not be initialized.");
                return;
            }
            if(projectContexts.Length > 1)
            {
                Debug.LogWarning($"More than one objects of type {nameof(NOProjectContext)} have been found in Resources \"{ResourcesCorePath}\" folder. First result will be used.");
            }
            ProjectContext = projectContexts[0];
            Application.quitting += Dispose;

            ProjectContext.SetupContext();
            HandleScenesLoad();
        }

        private static void HandleScenesLoad()
        {
        }

        private static void Dispose()
        {
            Debug.Log("Project Dispose");
            ProjectContext.DisposeContext();
            ProjectContext = null;
            Application.quitting -= Dispose;
        }
    }
}