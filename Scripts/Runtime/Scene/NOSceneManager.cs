using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NiqonNO.Core.Scene
{
    public class NOSceneManager : NOManagerWithStateScriptableObject<NOSceneManagerState>
    {
        public static NOSceneManager SceneManagerInstance
        {
            get => RuntimeState.Instance as NOSceneManager;
            private set => RuntimeState.Instance = value;
        }

        [SerializeField, HideLabel]
        private NOSceneDependencyData SceneDependencyTree;

        private Dictionary<string, NOSceneContext>  LoadedScenes => RuntimeState.LoadedScenes;
        private SortedSet<string> SortedScenesToLoad
        {
            get => RuntimeState.SortedScenesToLoad;
            set => RuntimeState.SortedScenesToLoad = value;
        }
        private SortedSet<string> SortedScenesToUnload
        {
            get => RuntimeState.SortedScenesToUnload;
            set => RuntimeState.SortedScenesToUnload = value;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            SceneManagerInstance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        [Button]
        public void LoadScene(string scene)
        {
            SortedScenesToLoad = NOSceneDependencyData.GetSceneDependencies(scene, SortedScenesToLoad);
        }
        
        [Button]
        public void UnloadScene(string scene)
        {
            SortedScenesToUnload = NOSceneDependencyData.GetSceneDependencies(scene, SortedScenesToUnload);
        }
        
        /*private async Awaitable LoadSceneCommand()
        {
            await Awaitable.FromAsyncOperation(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
            // do some heavy math here
            return; // this will make the returned awaitable complete on a background thread
        }*/
        
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadMode)
        {
            SetupSceneContext(scene);
            SortedScenesToLoad.Remove(scene.name);
            
        }
        
        public override void Dispose()
        {
            LoadedScenes.Reverse().ForEach(context => context.Value.DisposeSceneContext());
            SceneManager.sceneLoaded += OnSceneLoaded;
            base.Dispose();
        }
        
        private void SetupSceneContext(UnityEngine.SceneManagement.Scene scene)
        {
            var contextFound = false;
            foreach (var rootObject in scene.GetRootGameObjects())
            {
                if (!rootObject.TryGetComponent(out NOSceneContext context)) continue;
                
                if (contextFound)
                {
                    Debug.LogWarning($"More than one objects of type {nameof(NOSceneContext)} have been found on \"{scene.name}\" scene.");
                }

                contextFound = true;
                context.SetupSceneContext();
                LoadedScenes.Add(scene.name, context);
            }

            if (contextFound) return;
            LoadedScenes.Add(scene.name, null);
            Debug.LogWarning($"No object of type {nameof(NOSceneContext)} have been found on \"{scene.name}\" scene.");
        }
        
        private void DisposeSceneContext(UnityEngine.SceneManagement.Scene scene)
        {
            if (!LoadedScenes.TryGetValue(scene.name, out var context) || context == null)
            {
                Debug.LogWarning($"Could not find {nameof(NOSceneContext)} for \"{scene.name}\" scene to dispose.");
                return;
            }
            LoadedScenes[scene.name].DisposeSceneContext();
            LoadedScenes.Remove(scene.name);
        }
    }
}