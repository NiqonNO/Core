using Sirenix.OdinInspector;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Events;

namespace NiqonNO.Core
{
    public abstract class NOReference<T> : NOScriptableObject, INOValue<T>
    {
        [SerializeField, HideLabel, InlineProperty, DisableInInlineEditors] 
        protected T LocalValue;
        [SerializeField] 
        private CallbackType CallbackType;
        [SerializeField] 
        private UnityEvent<T> OnValueChange;
        
        [CreateProperty]
        public virtual T Value
        {
            get => LocalValue;
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
