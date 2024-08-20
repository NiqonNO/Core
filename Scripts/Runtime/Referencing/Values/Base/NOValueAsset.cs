using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOValueAsset<T> : NOScriptableObject
    {
        [SerializeField] 
        protected T LocalValue;
        public T Value => LocalValue;
    }
}
