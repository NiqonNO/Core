using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public abstract class NOValueBase<T1, T2>  where T2 : NOValueAsset<T1>
    {
        [SerializeField] 
        protected bool UseReference;
        [SerializeField] 
        protected T1 LocalValue;
        [SerializeField] 
        protected T2 LocalReference;
        public T1 Value => UseReference ? LocalReference.Value : LocalValue;

        protected NOValueBase(T1 value)
        {
            LocalValue = value;
        }
    }
    
    [Serializable] public abstract class NOValue<T> : NOValueBase<T, NOValueAsset<T>>
    {
        protected NOValue(T value) : base(value) { }
    }
}