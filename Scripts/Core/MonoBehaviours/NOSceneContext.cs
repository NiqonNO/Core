using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using NiqonNO.Core.Scene;
using NiqonNO.Core.Utility;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NOSceneContext : NOMonoBehaviour
    {
        [field: SerializeField]
        public NOManagerMonoBehaviour[] MonoBehaviourManagers { get; private set; }
        [field: SerializeField]
        public NOManagerScriptableObject[] ScriptableObjectManagers { get; private set; }

        IEnumerable<string> GetScenes() => NOUtility.GetScenesInBuildSettings();
        [field: SerializeField, ValueDropdown(nameof(GetScenes))]
        public string[] SceneDependencies { get; private set; }

        private void Awake()
        {
            NOSceneManager.SceneManager.RegisterScene(this);
        }
        void Start() => Debug.Log($"Scene {gameObject.scene.name} Start");
        
        public void SetupSceneContext()
        {
            Debug.Log($"Scene {gameObject.scene.name} Context Initialize");
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
            Debug.Log($"Scene {gameObject.scene.name} Context Dispose");
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