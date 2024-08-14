using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOManagerScriptableObject : NOScriptableObject, INOManager
    {
        public abstract void Initialize();
        public abstract void Dispose();
    }
}
