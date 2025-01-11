using System;
using UnityEngine;

namespace NiqonNO.Core.Utility.Attributes
{
    public class NOValueMaxAttribute : Attribute
    {
        public readonly float MaxValue;
        
        public NOValueMaxAttribute(float maxValue)
        {
            this.MaxValue = maxValue;
        }
    }
}
