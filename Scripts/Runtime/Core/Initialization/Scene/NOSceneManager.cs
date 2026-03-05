using System.Collections.Generic;
using System.Linq;
using NiqonNO.Core.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace NiqonNO.Core.Scene
{
    public class NOSceneManager : NOManagerWithStateSO<NOSceneManagerState>
    {
        [SerializeField, ValueDropdown(nameof(GetScenes))]
        public string MainScene;
        
        [SerializeField, HideLabel]
        private NOSceneDependencyData SceneDependencyTree;

        [Space]
        [SerializeField] 
        private UnityEvent OnLoadingStartedEvent;
        [SerializeField] 
        private UnityEvent OnLoadingFinishedEvent;

        private Dictionary<UnityEngine.SceneManagement.Scene, NOSceneContext>  LoadedScenes => RuntimeState.LoadedScenes;
        private bool IsLoading => LoadSceneCommand != null;
        
        private NOSceneLoadCommand LoadSceneCommand
        {
            get => RuntimeState.LoadSceneCommand;
            set => RuntimeState.LoadSceneCommand = value;
        }
        
        public override void Initialize()
        {
            base.Initialize();
#if UNITY_EDITOR
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                LoadScene(SceneManager.GetSceneAt(i).name);
            }
#else
            SceneManager.sceneLoaded += WaitForBootstrap;
#endif
        }

        private void WaitForBootstrap(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadMode)
        {
            SceneManager.sceneLoaded -= WaitForBootstrap;
            LoadScene(MainScene);
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
        
        private void SetupSceneContext(UnityEngine.SceneManagement.Scene scene)// => NOContainer.SetupSceneContext(scene);
        {
            if (LoadedScenes.ContainsKey(scene)) return;
            
            NOSceneContext context = null;
            foreach (var rootObject in scene.GetRootGameObjects())
            {
                if (!rootObject.TryGetComponent(out context)) continue;
                
                if (context.MainScene) SceneManager.SetActiveScene(scene);
                context.InitializeContext();
                break;
            }

            LoadedScenes.Add(scene, context);
        }
        
        private void DisposeSceneContext(UnityEngine.SceneManagement.Scene scene)// => NOContainer.DisposeSceneContext(scene);
        {
            if (!LoadedScenes.TryGetValue(scene, out var context)) return;
            
            if (context != null)
            {
                context.DisposeContext();
            }
            
            LoadedScenes.Remove(scene);
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
                scene.Value.DisposeContext();
            }

            base.Dispose();
        }
        
        private IEnumerable<string> GetScenes => NOSceneUtility.GetScenesInBuildSettings();
    }
}