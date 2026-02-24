using System;
using UnityEngine;
using UnityEngine.Events;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOVector2Reference : NOReference<Vector2>
    {
        [SerializeField] 
        private UnityEvent<float> OnValueChangeX;
        [SerializeField] 
        private UnityEvent<float> OnValueChangeY;
        
        protected override void OnValueUpdated()
        {
            base.OnValueUpdated();
            OnValueChangeX?.Invoke(LocalValue.x);
            OnValueChangeY?.Invoke(LocalValue.y);
        }
        protected override void OnValueChanged(Vector2 oldVal)
        {
            base.OnValueChanged(oldVal);
            if (!LocalValue.x.Equals(oldVal.x)) OnValueChangeX?.Invoke(LocalValue.x);
            if (!LocalValue.y.Equals(oldVal.y)) OnValueChangeY?.Invoke(LocalValue.y);
        }
    }
}
