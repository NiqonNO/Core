using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine.SceneManagement;

namespace NiqonNO.Core.Utility.Editor
{
    public static class NOEditorUtility
    {
        public static IEnumerable<string> GetScenesInBuildSettings()
        {
            HashSet<string> sceneNames = new();
            foreach (var sceneBuildData in UnityEditor.EditorBuildSettings.scenes)
            {
                sceneNames.Add(System.IO.Path.GetFileNameWithoutExtension(sceneBuildData.path));
            }
            return sceneNames;
        }
    }
}