using System;
using Sirenix.OdinInspector;
using Unity.Properties;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] 
    public abstract class NOValue<T> : INOValue<T>
    {
        [SerializeField] 
        protected bool UseReference;
        [SerializeField, HideLabel, InlineProperty] 
        protected T LocalValue;
        [SerializeField, HideLabel, InlineEditor] 
        protected NOReference<T> LocalReference;
        
        [CreateProperty]
        public T Value
        {
            get
            {
                if (!UseReference)
                {
                    return LocalValue;
                }
                if (LocalReference)
                {
                    return LocalReference.Value;
                }
                UseReference = false;
                return LocalValue;
            }
            set
            {
                if (!UseReference)
                {
                    LocalValue = value;
                }
                else if (LocalReference)
                {
                    LocalReference.Value = value;
                }
                else
                {
                    UseReference = false;
                    LocalValue = value;
                }
            }
        }

        protected NOValue()
        {
            LocalValue = default;
        }
        protected NOValue(T value) 
        {
            LocalValue = value;
        }
        protected NOValue(NOReference<T> value)
        {
            UseReference = true;
            LocalReference = value;
        }
    }
}
