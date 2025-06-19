using System;
using UnityEngine;
using UnityEngine.Events;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOColorVariableAsset : NOVariableAsset<Color>
    {
        [SerializeField] 
        private UnityEvent<float> OnValueChangeR;
        [SerializeField] 
        private UnityEvent<float> OnValueChangeG;
        [SerializeField] 
        private UnityEvent<float> OnValueChangeB;
        [SerializeField] 
        private UnityEvent<float> OnValueChangeA;
            
        protected override void OnValueUpdated()
        {
            base.OnValueUpdated();
            OnValueChangeR?.Invoke(LocalValue.r);
            OnValueChangeG?.Invoke(LocalValue.g);
            OnValueChangeB?.Invoke(LocalValue.b);
            OnValueChangeA?.Invoke(LocalValue.a);
        }
        protected override void OnValueChanged(Color oldVal)
        {
            base.OnValueChanged(oldVal);
            if (!LocalValue.r.Equals(oldVal.r)) OnValueChangeR?.Invoke(LocalValue.r);
            if (!LocalValue.g.Equals(oldVal.g)) OnValueChangeG?.Invoke(LocalValue.g);
            if (!LocalValue.b.Equals(oldVal.b)) OnValueChangeB?.Invoke(LocalValue.b);
            if (!LocalValue.a.Equals(oldVal.a)) OnValueChangeA?.Invoke(LocalValue.a);
        }
    }
}