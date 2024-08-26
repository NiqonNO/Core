using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public abstract class NOVariableBase<T1, T2> : NOValueBase<T1, T2> where T2 : NOVariableAsset<T1> 
    {
        public new T1 Value
        {
            get => base.Value;
            set
            {
                if(UseReference) LocalReference.Value = value;
                else LocalValue = value;
            }
        }

        protected NOVariableBase(T1 value) : base(value) { }
    }
    
    [Serializable] public abstract class NOVariable<T> : NOVariableBase<T, NOVariableAsset<T>>
    {
        protected NOVariable(T value) : base(value) { }
    }
}
