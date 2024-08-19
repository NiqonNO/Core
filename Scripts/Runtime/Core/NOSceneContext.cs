using System;
using Sirenix.Utilities;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NOSceneContext : NOMonoBehaviour
    {
        [field: SerializeField]
        public NOManagerMonoBehaviour[] MonoBehaviourManagers { get; private set; }
        [field: SerializeField]
        public NOManagerScriptableObject[] ScriptableObjectManagers { get; private set; }
        
        public void SetupSceneContext()
        {
            if (!ScriptableObjectManagers.IsNullOrEmpty())
            {
                ScriptableObjectManagers.ForEach(m => m.Initialize());
            }
            if (!MonoBehaviourManagers.IsNullOrEmpty())
            {
                MonoBehaviourManagers.ForEach(m => m.Initialize());
            }
        }

        public void DisposeSceneContext()
        {
            if (!MonoBehaviourManagers.IsNullOrEmpty())
            {
                MonoBehaviourManagers.ForEach(m => m.Dispose());
            }
            if (!ScriptableObjectManagers.IsNullOrEmpty())
            {
                ScriptableObjectManagers.ForEach(m => m.Dispose());
            }
        }
    }
}