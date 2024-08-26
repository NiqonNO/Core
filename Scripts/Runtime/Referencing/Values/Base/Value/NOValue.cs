using System;

namespace NiqonNO.Core
{
    [Serializable] public abstract class NOValue<T> : NOValueBase<T, NOValueAsset<T>>, INOValue<T>
    {
        public T Value => UseReference ? LocalReference.Value : LocalValue;
        
        protected NOValue(T value) : base(value) { }
    }
}