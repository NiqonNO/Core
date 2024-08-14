using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOManagerMonoBehaviour : MonoBehaviour, INOManager
    {
        public abstract void Initialize();
        public abstract void Dispose();
    }
}