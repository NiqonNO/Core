using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core.Scene
{
    public class NOSceneManagerState : NOManagerState
    {
        public HashSet<NOSceneContext> LoadedScenes = new();
    }
}
