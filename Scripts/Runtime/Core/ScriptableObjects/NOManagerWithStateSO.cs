using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOManagerWithStateSO<TRuntimeState> : NOManagerSO where TRuntimeState : NOManagerState 
    {
        [ReadOnly, ShowInInspector, PropertyOrder(float.MinValue)]
        protected TRuntimeState RuntimeState { get; private set; }

        public override void Initialize()
        {
            RuntimeState = CreateAsset<TRuntimeState>();
            RuntimeState.name = $"{GetType().Name} Runtime State";
        }
        
        public override void Dispose()
        {
            DestroyAsset(RuntimeState);
            RuntimeState = null;
        }
        
        protected T CreateAsset<T>() where T : NOScriptableObject
        {
            T item = CreateInstance<T>();
            NOContainer.Inject(item);
            return item;
        }
        protected void DestroyAsset<T>(T item) where T : NOScriptableObject
        {
            NOContainer.Release(item);
            Destroy(item);
        }
    }
}