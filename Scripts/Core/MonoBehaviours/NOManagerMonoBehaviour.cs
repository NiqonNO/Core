using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOManagerMonoBehaviour : NOMonoBehaviour, INOManager
    {
        public abstract void Initialize();
        public abstract void Dispose();
    }
}