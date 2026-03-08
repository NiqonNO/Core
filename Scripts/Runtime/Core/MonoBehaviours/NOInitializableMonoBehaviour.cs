namespace NiqonNO.Core
{
    public abstract class NOInitializableMonoBehaviour : NOMonoBehaviour, IInitializable
    {
        protected bool Initialized { get; private set; }

        private void Awake()
        {
            NOContainer.EnqueueForInitialization(gameObject.scene.name, this);
        }

        public void Initialize()
        {
            Initialized = true;
            OnGameReady();
        }

        protected virtual void OnGameReady()
        {
            
        }
    }
}