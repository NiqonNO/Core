using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace NiqonNO.Core.Scene
{
    public class NOSceneManagerState : NOManagerState<NOSceneManager>
    {
        [ReadOnly, ShowInInspector]
        public Dictionary<string, NOSceneContext> LoadedScenes = new();

        [ReadOnly, ShowInInspector] 
        public NOSceneLoadCommand LoadSceneCommand;
    }
}