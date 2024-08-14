using NiqonNO.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NOProjectContext : NOScriptableObject, INOContext
    {
        [field: SerializeField]
        public NOManagerScriptableObject[] ScriptableObjectManagers { get; private set; }

        IEnumerable<string> GetScenes() => NOUtility.GetScenesInBuildSettings();
        [field: SerializeField, ValueDropdown(nameof(GetScenes))]
        public string MainScene { get; private set; }

        public void SetupContext()
        {
            Debug.Log("Project Context Initialize");
            if (!ScriptableObjectManagers.IsNullOrEmpty())
            {
                ScriptableObjectManagers.ForEach(m => m.Initialize());
            }
        }
        public void DisposeContext()
        {
            Debug.Log("Project Context Dispose");
            if (!ScriptableObjectManagers.IsNullOrEmpty())
            {
                ScriptableObjectManagers.ForEach(m => m.Dispose());
            }
        }
    }
}