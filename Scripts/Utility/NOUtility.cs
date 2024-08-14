using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Utility
{
    public static class NOUtility
    {
        public static IEnumerable<string> GetScenesInBuildSettings()
        {
            HashSet<string> sceneNames = new();
#if UNITY_EDITOR
            foreach (var scene in UnityEditor.EditorBuildSettings.scenes)
            {
                sceneNames.Add(System.IO.Path.GetFileNameWithoutExtension(scene.path));
            }
#else
            foreach (var scene in UnityEngine.SceneManagement.SceneManager.GetAllScenes())
            {
                sceneNames.Add(scene.name);
            }
#endif
            return sceneNames;
        }
    }
}