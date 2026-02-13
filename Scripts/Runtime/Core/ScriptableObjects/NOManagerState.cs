using Sirenix.OdinInspector;

namespace NiqonNO.Core
{
    public abstract class NOManagerState<T> : NOScriptableObject where T : INOManager
    {
        [ReadOnly, ShowInInspector, PropertyOrder(-1000)]
        public T Instance;
    }
}