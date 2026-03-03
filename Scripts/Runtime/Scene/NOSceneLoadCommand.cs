using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NiqonNO.Core.Scene
{
    public class NOSceneLoadCommand
    {
        private List<string> SortedScenesToLoad;
        private List<string> SortedScenesToUnload;

        private readonly Action OnLoadingFinished;
        private AsyncOperation SceneLoadingOperation;

        public NOSceneLoadCommand(string sceneToLoad, string sceneToUnload, Action onLoadingFinished)
        {
            SortedScenesToLoad = NOSceneDependencyData.GetSceneDependencies(sceneToLoad, false);
            SortedScenesToUnload = NOSceneDependencyData.GetSceneDependencies(sceneToUnload, true);
            OnLoadingFinished = onLoadingFinished;
        }

        public event Action<UnityEngine.SceneManagement.Scene> OnSceneLoaded;
        public event Action<UnityEngine.SceneManagement.Scene> OnBeforeSceneUnloaded;

        public void AddSceneToLoad(string sceneToLoad) => SortedScenesToLoad =
            NOSceneDependencyData.GetSceneDependencies(sceneToLoad, SortedScenesToLoad, false);

        public void AddSceneToUnload(string sceneToUnload) => SortedScenesToUnload =
            NOSceneDependencyData.GetSceneDependencies(sceneToUnload, SortedScenesToUnload, true);

        public void Run()
        {
            LoadSceneCommand();
        }

        public void Complete()
        {
        }

        public void Cancel() => Complete();

        private void LoadSceneCommand()
        {
            if (SortedScenesToUnload.Count > 0)
            {
                UnloadScene(SortedScenesToUnload[0]);
                return;
            }

            if (SortedScenesToLoad.Count > 0)
            {
                LoadScene(SortedScenesToLoad[0]);
                return;
            }

            OnLoadingFinished.Invoke();
        }

        void LoadScene(string scene)
        {
            if (SceneManager.GetSceneByName(scene).isLoaded)
            {
                SortedScenesToLoad.Remove(scene);
                OnSceneLoaded?.Invoke(SceneManager.GetSceneByName(scene));
                LoadSceneCommand();
                return;
            }

            SceneLoadingOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            SceneLoadingOperation.completed += Completed;

            void Completed(AsyncOperation op)
            {
                OnSceneLoaded?.Invoke(SceneManager.GetSceneByName(scene));
                SortedScenesToLoad.Remove(scene);
                LoadSceneCommand();
            }
        }
        void UnloadScene(string scene)
        {
            if (!SceneManager.GetSceneByName(scene).isLoaded)
            {
                OnBeforeSceneUnloaded?.Invoke( SceneManager.GetSceneByName(scene));
                SortedScenesToUnload.Remove(scene);
                LoadSceneCommand();
                return;
            }
            if (SceneManager.sceneCount == 1)
            {
                Debug.LogWarning($"Could not unload scene {scene}, as it is last scene");
                SortedScenesToUnload.Remove(scene);
                LoadSceneCommand();
                return;
            }
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (!NOSceneDependencyData.SceneDependsOn(SceneManager.GetSceneAt(i).name, scene)) continue;
                    
                Debug.LogWarning($"Could not unload scene {scene}, as scene {SceneManager.GetSceneAt(i).name} depends on it");
                SortedScenesToUnload.Remove(scene);
                LoadSceneCommand();
                return;
            }
            
            OnBeforeSceneUnloaded?.Invoke(SceneManager.GetSceneByName(scene));
            SceneLoadingOperation = SceneManager.UnloadSceneAsync(scene);
            SceneLoadingOperation.completed += Completed;
            
            void Completed(AsyncOperation op)
            {
                SortedScenesToUnload.Remove(scene);
                LoadSceneCommand();
            };
        }
    }
}