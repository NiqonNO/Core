using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOManagerWithStateScriptableObject<T> : NOManagerScriptableObject where T : NOManagerState
    {
        protected T RuntimeState { get; private set; }

        public override void Initialize()
        {
            RuntimeState = CreateInstance<T>();
        }

        public override void Dispose()
        {
            Destroy(RuntimeState);
            RuntimeState = null;
        }
    }
}