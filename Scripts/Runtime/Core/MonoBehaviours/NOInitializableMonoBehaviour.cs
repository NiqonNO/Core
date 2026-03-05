namespace NiqonNO.Core
{
    public abstract class NOInitializableMonoBehaviour : NOMonoBehaviour, IInitializable
    {
        private void Awake()
        {
            NOContainer.EnqueueForInitialization(gameObject.scene.name, this);
        }

        public abstract void Initialize();
    }
}