using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public abstract class NOVariable<T> : NOValueBase<T, NOVariableAsset<T>>, INOVariable<T>
    {
        public T Value
        {
            get => UseReference ? LocalReference.Value : LocalValue;
            set
            {
                if(UseReference) LocalReference.Value = value;
                else LocalValue = value;
            }
        }
        protected NOVariable(T value) : base(value) { }
    }
}
