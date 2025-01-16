using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public abstract class NOVariable<T> : NOValueBase<T, NOVariableAsset<T>>, INOVariable<T>
    {
        public T Value
        {
            get => UseReference ? LocalReference.Value?? default : LocalValue;
            set
            {
                if(!UseReference) LocalValue = value;
                else if(LocalReference) LocalReference.Value = value;
            }
        }
        protected NOVariable(T value) : base(value) { }
    }
}
