using NiqonNO.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NOSceneContext : MonoBehaviour, INOContext
    {
        [field: SerializeField]
        public NOManagerMonoBehaviour[] MonoBehaviourManagers { get; private set; }
        [field: SerializeField]
        public NOManagerScriptableObject[] ScriptableObjectManagers { get; private set; }

        IEnumerable<string> GetScenes() => NOUtility.GetScenesInBuildSettings();
        [field: SerializeField, ValueDropdown(nameof(GetScenes))]
        public string[] SceneDependencies { get; private set; }

        public void SetupContext()
        {
            Debug.Log($"Scene {gameObject.scene.name} Context Initialize");
            if (!MonoBehaviourManagers.IsNullOrEmpty())
            {
                MonoBehaviourManagers.ForEach(m => m.Initialize());
            }
            if (!ScriptableObjectManagers.IsNullOrEmpty())
            {
                ScriptableObjectManagers.ForEach(m => m.Initialize());
            }
        }

        void Awake() => Debug.Log($"Scene {gameObject.scene.name} Awake");
        void Start() => Debug.Log($"Scene {gameObject.scene.name} Start");
        void OnDisable() => Debug.Log($"Scene {gameObject.scene.name} Disable");
        void OnDestroy() => Debug.Log($"Scene {gameObject.scene.name} Destroy");

        public void DisposeContext()
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