namespace NiqonNO.Core
{
    public abstract class NOInitializableMonoBehaviour : NOMonoBehaviour, IInitializable
    {
        private void Awake()
        {
            NOContextLocator.EnqueueForInitialization(gameObject.scene, this);
        }

        public abstract void Initialize();
    }
}