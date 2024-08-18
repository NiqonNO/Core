using System;
using System.Collections.Generic;
using NiqonNO.Core.Scene;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace NiqonNO.Core.Editor
{
    [InitializeOnLoad]
    public static class NOEditorSceneHandler
    {
        static NOEditorSceneHandler()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorBuildSettings.sceneListChanged -= OnSceneListChanged;
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
            OnSceneListChanged();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            switch(stateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    LoadSceneDependencies();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stateChange), stateChange, null);
            }
        }

        private static void LoadSceneDependencies()
        {
            SortedSet<string> sortedScenesToOpen = null;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                sortedScenesToOpen = sortedScenesToOpen == null ? 
                    NOSceneDependencyData.GetSceneDependencies(SceneManager.GetSceneAt(i).name) : 
                    NOSceneDependencyData.GetSceneDependencies(SceneManager.GetSceneAt(i).name, sortedScenesToOpen);
            }
            if (sortedScenesToOpen == null) return;
            string lastScene = default;
            foreach (var scene in sortedScenesToOpen)
            {
                OpenScene(scene);
                lastScene = scene;
            }

            return;

            void OpenScene(string toLoad)
            {
                if (!SceneManager.GetSceneByName(toLoad).isLoaded)
                {
                    EditorSceneManager.OpenScene(GetScenePath(toLoad), OpenSceneMode.Additive);
                }
                else if (lastScene != default)
                {
                    EditorSceneManager.MoveSceneAfter(SceneManager.GetSceneByName(toLoad), SceneManager.GetSceneByName(lastScene));
                }
                
                if (lastScene == default)
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(toLoad));
                }
            }
            string GetScenePath(string toLoad)
            {
                foreach (var sceneBuildData in EditorBuildSettings.scenes)
                {
                    if (System.IO.Path.GetFileNameWithoutExtension(sceneBuildData.path) != toLoad) continue;
                    return sceneBuildData.path;
                }
                throw new ArgumentNullException($"Scene {toLoad} is not present in EditorBuildSettings scene list.");
            }
        }

        private static void OnSceneListChanged()
        {
            NOSceneDependencyData.ValidateData();
        }
    }
}
