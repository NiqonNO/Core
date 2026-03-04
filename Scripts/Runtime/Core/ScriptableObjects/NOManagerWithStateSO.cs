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
            RuntimeState = Context.Factory.CreateAsset<TRuntimeState>();
            RuntimeState.name = $"{GetType().Name} Runtime State";
        }
        
        public override void Dispose()
        {
            Destroy(RuntimeState);
            RuntimeState = null;
        }
    }
}