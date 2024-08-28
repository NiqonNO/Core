using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOValueAsset<T> : NOScriptableObject, INOValue<T>
    {
        [SerializeField, HideLabel, InlineProperty, DisableInInlineEditors] 
        protected T LocalValue;
        public T Value => LocalValue;
    }
}
