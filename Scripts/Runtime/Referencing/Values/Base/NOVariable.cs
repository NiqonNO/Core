using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOVariable<T1, T2> : NOValue<T1, T2> where T2 : NOVariableAsset<T1> 
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
    }
}
