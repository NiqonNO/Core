using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NiqonNO.Core.Scene
{
    public class NOSceneLoadCommand : IDisposable
    {
        private SortedSet<string> SortedScenesToLoad = new(new NOSceneDependencyData.SceneDepthComparer(true));
        private SortedSet<string> SortedScenesToUnload = new(new NOSceneDependencyData.SceneDepthComparer(false));
        
        private readonly Action OnLoadingFinished;
        private CancellationTokenSource LoadSceneCommandCancellationToken;
        
        public NOSceneLoadCommand(string sceneToLoad, string sceneToUnload, Action onLoadingFinished)
        {
            SortedScenesToLoad = NOSceneDependencyData.GetSceneDependencies(sceneToLoad, SortedScenesToLoad);
            SortedScenesToUnload = NOSceneDependencyData.GetSceneDependencies(sceneToUnload, SortedScenesToUnload);
            OnLoadingFinished = onLoadingFinished;
        }

        public event Action<UnityEngine.SceneManagement.Scene> OnSceneLoaded;
        public event Action<UnityEngine.SceneManagement.Scene> OnBeforeSceneUnloaded;

        public void AddSceneToLoad(string sceneToLoad) => SortedScenesToLoad = NOSceneDependencyData.GetSceneDependencies(sceneToLoad, SortedScenesToLoad);
        public void AddSceneToUnload(string sceneToUnload) => SortedScenesToUnload = NOSceneDependencyData.GetSceneDependencies(sceneToUnload, SortedScenesToUnload);
        public void Run()
        {
            LoadSceneCommand();
        }
        public void Complete()
        {
            LoadSceneCommandCancellationToken?.Dispose();
            LoadSceneCommandCancellationToken = null;
        }
        public void Cancel()
        {
            LoadSceneCommandCancellationToken?.Cancel();
        }
        void IDisposable.Dispose() => Cancel();

        private async void LoadSceneCommand()
        {
            LoadSceneCommandCancellationToken = new CancellationTokenSource();
            while (SortedScenesToLoad.Count + SortedScenesToUnload.Count > 0)
            {
                if (SortedScenesToUnload.Any()) await UnloadScene(SortedScenesToUnload.First());
                else if (SortedScenesToLoad.Any()) await LoadScene(SortedScenesToLoad.First());
            }
            OnLoadingFinished.Invoke();
            return;
            
            async Awaitable LoadScene(string scene)
            {
                if (SceneManager.GetSceneByName(scene).isLoaded)
                {
                    SortedScenesToLoad.Remove(scene);
                    return;
                }
                await Awaitable.FromAsyncOperation(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
                OnSceneLoaded?.Invoke(SceneManager.GetSceneByName(scene));
                SortedScenesToLoad.Remove(scene);
            }
            async Awaitable UnloadScene(string scene)
            {
                if (!SceneManager.GetSceneByName(scene).isLoaded)
                {
                    SortedScenesToUnload.Remove(scene);
                    return;
                }
                if (SceneManager.sceneCount == 1)
                {
                    Debug.LogWarning($"Could not unload scene {scene}, as it is last scene");
                    SortedScenesToUnload.Remove(scene);
                    return;
                }
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    if (!NOSceneDependencyData.SceneDependsOn(SceneManager.GetSceneAt(i).name, scene)) continue;
                    
                    Debug.LogWarning($"Could not unload scene {scene}, as scene {SceneManager.GetSceneAt(i).name} depends on it");
                    SortedScenesToUnload.Remove(scene);
                    return;
                }
                OnBeforeSceneUnloaded?.Invoke(SceneManager.GetSceneByName(scene));
                await Awaitable.FromAsyncOperation(SceneManager.UnloadSceneAsync(scene));
                SortedScenesToUnload.Remove(scene);
            }
        }
    }
}