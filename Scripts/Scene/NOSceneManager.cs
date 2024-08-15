using System.Collections.Generic;
using NiqonNO.Core.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NiqonNO.Core.Scene
{
    public class NOSceneManager : NOManagerWithStateScriptableObject<NOSceneManagerState>
    {
        public static NOSceneManager SceneManager
        {
            get => RuntimeState.Instance as NOSceneManager;
            private set => RuntimeState.Instance = value;
        }

        IEnumerable<string> GetScenes() => NOUtility.GetScenesInBuildSettings();
        [field: SerializeField, ValueDropdown(nameof(GetScenes))]
        public string MainScene { get; private set; }
        
        private HashSet<NOSceneContext> LoadedScenes => RuntimeState.LoadedScenes;
        
        public override void Initialize()
        {
            base.Initialize();
            SceneManager = this;
            //SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadMode)
        {
            //Debug.Log($"Scene {scene.name} Loaded");
        }
        
        public override void Dispose()
        {
            //SceneManager.sceneLoaded -= OnSceneLoaded;
            LoadedScenes.ForEach(context => context.DisposeSceneContext());
            SceneManager = null;
            base.Dispose();
        }

        public void RegisterScene(NOSceneContext sceneContext)
        {
            if (!LoadedScenes.Add(sceneContext))
                return;
            sceneContext.SetupSceneContext();
        }
    }
}