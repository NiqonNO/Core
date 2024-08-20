using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public abstract class NOValue<T1, T2>  where T2 : NOValueAsset<T1>
    {
        [SerializeField] 
        protected bool UseReference;
        [SerializeField] 
        protected T1 LocalValue;
        [SerializeField] 
        protected T2 LocalReference;
        public T1 Value => UseReference ? LocalReference.Value : LocalValue;
    }
}