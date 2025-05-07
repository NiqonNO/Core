using UnityEngine;

namespace NiqonNO.Core.MVVM
{
    public class NOMVVMBoolBinder : NOMVVMBinder<bool>
    {
        [SerializeField] 
        private bool InvertValue;
        protected override bool ProcessValue(bool rawValue) => rawValue ^ InvertValue;
    }
}