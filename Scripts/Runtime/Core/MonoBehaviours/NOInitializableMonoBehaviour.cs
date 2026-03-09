using System;

namespace NiqonNO.Core
{
    public abstract class NOInitializableMonoBehaviour : NOMonoBehaviour, IInitializable
    {
        protected bool Initialized { get; private set; }
        protected event Action OnGameReadyEvent;

        protected virtual void Awake()
        {
            NOContainer.EnqueueForInitialization(gameObject.scene.name, this);
        }

        public void Initialize()
        {
            Initialized = true;
            OnGameReadyEvent?.Invoke();
            OnGameReadyEvent = null;
        }
    }
}