using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace NiqonNO.Core.Scene
{
    public class NOSceneManagerState : NOManagerState
    {
        [ReadOnly, ShowInInspector]
        public Dictionary<UnityEngine.SceneManagement.Scene, NOSceneContext> LoadedScenes = new();

        [ReadOnly, ShowInInspector] 
        public NOSceneLoadCommand LoadSceneCommand;
    }
}