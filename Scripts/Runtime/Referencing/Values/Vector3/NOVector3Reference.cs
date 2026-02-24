using System;
using UnityEngine;
using UnityEngine.Events;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOVector3Reference : NOReference<Vector3>
    {
            [SerializeField] 
            private UnityEvent<float> OnValueChangeX;
            [SerializeField] 
            private UnityEvent<float> OnValueChangeY;
            [SerializeField] 
            private UnityEvent<float> OnValueChangeZ;
            
            protected override void OnValueUpdated()
            {
                base.OnValueUpdated();
                OnValueChangeX?.Invoke(LocalValue.x);
                OnValueChangeY?.Invoke(LocalValue.y);
                OnValueChangeZ?.Invoke(LocalValue.z);
            }
            protected override void OnValueChanged(Vector3 oldVal)
            {
                base.OnValueChanged(oldVal);
                if (!LocalValue.x.Equals(oldVal.x)) OnValueChangeX?.Invoke(LocalValue.x);
                if (!LocalValue.y.Equals(oldVal.y)) OnValueChangeY?.Invoke(LocalValue.y);
                if (!LocalValue.z.Equals(oldVal.z)) OnValueChangeZ?.Invoke(LocalValue.z);
            }
    }
}
