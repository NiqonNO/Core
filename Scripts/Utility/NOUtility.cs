using System.Collections.Generic;

namespace NiqonNO.Core.Utility
{
    public static class NOUtility
    {
        public static IEnumerable<string> GetScenesInBuildSettings()
        {
            HashSet<string> sceneNames = new();
#if UNITY_EDITOR
            foreach (var sceneBuildData in UnityEditor.EditorBuildSettings.scenes)
            {
                sceneNames.Add(System.IO.Path.GetFileNameWithoutExtension(sceneBuildData.path));
            }
#endif
            return sceneNames;
        }
    }
}