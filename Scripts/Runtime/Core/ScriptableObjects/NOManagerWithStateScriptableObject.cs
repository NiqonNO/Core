using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOManagerWithStateScriptableObject<T> : NOManagerScriptableObject where T : NOManagerState
    {
        [ReadOnly, ShowInInspector, PropertyOrder(-1000)]
        protected static T RuntimeState { get; private set; }

        public override void Initialize()
        {
            if (RuntimeState)
            {
                Debug.LogError($"More than one manager of type {GetType().Name} is initialized or previous session have not been cleared correctly.", this);
                return;
            }
            RuntimeState = CreateInstance<T>();
            RuntimeState.name = $"{GetType().Name} Runtime State";
        }
        
        public override void Dispose()
        {
            Destroy(RuntimeState);
            RuntimeState = null;
        }
    }
}