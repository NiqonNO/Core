namespace NiqonNO.Core
{
    public abstract class NOManagerSO : NOScriptableObject, INOManager
    {
        public abstract void Initialize();
        public abstract void Dispose();
    }
}
