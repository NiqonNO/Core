using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Events;

namespace NiqonNO.Core
{
    public abstract class NOVariableAsset<T> : NOValueAsset<T>, INOVariable<T>
    {
        [CreateProperty]
        public new T Value
        {
            get => base.Value;
            set
            {
                switch (CallbackType)
                {
                    case CallbackType.OnValueChanged:
                        var oldVal = LocalValue;
                        LocalValue = value;
                        OnValueChanged(oldVal);
                        break;
                    case CallbackType.OnValueUpdate:
                        LocalValue = value;
                        OnValueUpdated();
                        break;
                    case CallbackType.None:
                    default:
                        LocalValue = value;
                        break;
   
                }
            }
        }
        
        [SerializeField] 
        private CallbackType CallbackType;
        [SerializeField] 
        private UnityEvent<T> OnValueChange;

        protected virtual void OnValueUpdated()
        {
            OnValueChange?.Invoke(LocalValue);
        }
        protected virtual void OnValueChanged(T oldVal)
        {
            if (!LocalValue.Equals(oldVal)) OnValueChange?.Invoke(LocalValue);
        }
    }
}
