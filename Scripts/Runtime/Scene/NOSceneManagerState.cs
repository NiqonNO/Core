using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core.Scene
{
    public class NOSceneManagerState : NOManagerState
    {
        [ReadOnly, ShowInInspector]
        public Dictionary<string, NOSceneContext> LoadedScenes = new();
        
        [ReadOnly, ShowInInspector]
        public SortedSet<string> SortedScenesToLoad = new(new NOSceneDependencyData.SceneDepthComparer(true));
        [ReadOnly, ShowInInspector]
        public SortedSet<string> SortedScenesToUnload = new(new NOSceneDependencyData.SceneDepthComparer(false));
    }
}