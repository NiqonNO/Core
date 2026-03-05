using System.Collections.Generic;
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

        private List<UnityEngine.SceneManagement.Scene>  LoadedScenes => RuntimeState.LoadedScenes;
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
            LoadSceneCommand.OnSceneLoaded -= SetupSceneContext;
            LoadSceneCommand.OnBeforeSceneUnloaded -= DisposeSceneContext;
            LoadSceneCommand = null;
        }
        
        private void SetupSceneContext(UnityEngine.SceneManagement.Scene scene)
        {
            if (LoadedScenes.Contains(scene)) return;
            LoadedScenes.Add(scene);
            NOContainer.SetupSceneContext(scene);
            if (scene.name.Equals(MainScene)) SceneManager.SetActiveScene(scene);
        }
        
        private void DisposeSceneContext(UnityEngine.SceneManagement.Scene scene)
        {
            if (!LoadedScenes.Remove(scene)) return;
            NOContainer.DisposeSceneContext(scene);
        }
        
        public override void Dispose()
        {
            if (IsLoading)
            {
                LoadSceneCommand.Cancel();
            }

            for (var i = LoadedScenes.Count - 1; i >= 0 ; i--)
            {
                NOContainer.DisposeSceneContext( LoadedScenes[i]);
            }

            base.Dispose();
        }
        
        private IEnumerable<string> GetScenes => NOSceneUtility.GetScenesInBuildSettings();
    }
}