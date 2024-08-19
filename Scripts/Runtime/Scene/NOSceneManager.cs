using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
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

        [Space]
        [SerializeField] 
        private UnityEvent OnLoadingStartedEvent;
        [SerializeField] 
        private UnityEvent OnLoadingFinishedEvent;

        private Dictionary<string, NOSceneContext>  LoadedScenes => RuntimeState.LoadedScenes;
        private bool IsLoading => LoadSceneCommand != null;
        
        private NOSceneLoadCommand LoadSceneCommand
        {
            get => RuntimeState.LoadSceneCommand;
            set => RuntimeState.LoadSceneCommand = value;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            SceneManagerInstance = this;
            SceneManager.sceneLoaded += CheckGameReady;
        }

        private void CheckGameReady(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadMode)
        {
            SetupSceneContext(scene);
            if (SceneManager.sceneCount != LoadedScenes.Count) return;
            SceneManager.sceneLoaded -= CheckGameReady;
        }

        [Button]
        public void SwitchScene(string sceneToLoad, string sceneToUnload)
        {
            if (IsLoading)
            {
                LoadSceneCommand.AddSceneToLoad(sceneToLoad);
                LoadSceneCommand.AddSceneToUnload(sceneToUnload);
                return;
            }
            CreateSceneLoadCommand(sceneToLoad, sceneToUnload);
        }
        [Button]
        public void LoadScene(string scene)
        {
            if (IsLoading)
            {
                LoadSceneCommand.AddSceneToLoad(scene);
                return;
            }
            CreateSceneLoadCommand(scene, "");
        }
        [Button]
        public void UnloadScene(string scene)
        {
            if (IsLoading)
            {
                LoadSceneCommand.AddSceneToUnload(scene);
                return;
            }
            CreateSceneLoadCommand("", scene);
        }

        private void CreateSceneLoadCommand(string sceneToLoad, string sceneToUnload)
        {
            OnLoadingStartedEvent.Invoke();
            LoadSceneCommand = new NOSceneLoadCommand(sceneToLoad, sceneToUnload, FinishSceneLoadCommand);
            LoadSceneCommand.OnSceneLoaded += SetupSceneContext;
            LoadSceneCommand.OnBeforeSceneUnloaded += DisposeSceneContext;
            LoadSceneCommand.Run();
        }
        private void FinishSceneLoadCommand()
        {
            OnLoadingFinishedEvent.Invoke();
            LoadSceneCommand.Complete();
            LoadSceneCommand.OnSceneLoaded -= SetupSceneContext;
            LoadSceneCommand.OnBeforeSceneUnloaded -= DisposeSceneContext;
            LoadSceneCommand = null;
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
            if (!LoadedScenes.TryGetValue(scene.name, out var context))
            {
                Debug.LogWarning($"Could not find {nameof(NOSceneContext)} for \"{scene.name}\" scene to dispose.");
                return;
            }
            if (context == null)
            {
                Debug.LogWarning($"{nameof(NOSceneContext)} for \"{scene.name}\" scene is missing.");
                LoadedScenes.Remove(scene.name);
                return;
            }
            
            context.DisposeSceneContext();
            LoadedScenes.Remove(scene.name);
        }
        
        public override void Dispose()
        {
            if (IsLoading)
            {
                LoadSceneCommand.Cancel();
            }

            foreach (var scene in LoadedScenes.Reverse())
            {
                if (!scene.Value) continue;
                scene.Value.DisposeSceneContext();
            }

            base.Dispose();
        }
    }
}