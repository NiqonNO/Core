using Sirenix.OdinInspector;

namespace NiqonNO.Core
{
    public abstract class NOManagerWithStateSO<TRuntimeState> : NOManagerSO where TRuntimeState : NOManagerState 
    {
        [ReadOnly, ShowInInspector, PropertyOrder(float.MinValue)]
        protected TRuntimeState RuntimeState { get; private set; }

        public bool Initialized { get; private set; }

        public override void Initialize()
        {
            RuntimeState = Context.Factory.CreateAsset<TRuntimeState>();
            RuntimeState.name = $"{GetType().Name} Runtime State";
            Initialized = true;
        }
        
        public override void Dispose()
        {
            Initialized = false;
            Destroy(RuntimeState);
            RuntimeState = null;
        }
    }
}