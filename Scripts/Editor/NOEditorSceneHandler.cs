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
            EditorBuildSettings.sceneListChanged -= OnSceneListChanged;
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
            OnSceneListChanged();
        }

        private static void OnSceneListChanged()
        {
            NOSceneDependencyData.ValidateData();
        }
    }
}
