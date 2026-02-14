using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOManagerWithStateScriptableObject<T1, T2> : NOManagerScriptableObject 
        where T1 : NOManagerScriptableObject 
        where T2 : NOManagerState<T1>
    {
        [ReadOnly, ShowInInspector, PropertyOrder(float.MinValue)]
        protected static T2 RuntimeState { get; private set; }
        protected static T1 Instance  => RuntimeState.Instance;

        public override void Initialize()
        {
            if (RuntimeState)
            {
                Debug.LogError($"More than one manager of type {GetType().Name} is initialized or previous session have not been cleared correctly.", this);
                return;
            }
            RuntimeState = CreateInstance<T2>();
            RuntimeState.name = $"{GetType().Name} Runtime State";
            RuntimeState.Instance = this as T1;
        }
        
        public override void Dispose()
        {
            Destroy(RuntimeState);
            RuntimeState.Instance = null;
            RuntimeState = null;
        }
    }
}