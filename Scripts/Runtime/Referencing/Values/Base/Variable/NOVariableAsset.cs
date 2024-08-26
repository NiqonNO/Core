using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOVariableAsset<T> : NOValueAsset<T>, INOVariable<T>
    {
        public new T Value
        {
            get => base.Value;
            set => LocalValue = value;
        }
    }
}
