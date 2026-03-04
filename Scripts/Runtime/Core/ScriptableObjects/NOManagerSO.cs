namespace NiqonNO.Core
{
    public abstract class NOManagerSO : NOScriptableObject, INOManager
    {
        [NOInject] 
        protected INOContext Context;
        
        public abstract void Initialize();
        public abstract void Dispose();
    }
}
