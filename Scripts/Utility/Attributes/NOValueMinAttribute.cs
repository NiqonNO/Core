using System;
using UnityEngine;

namespace NiqonNO.Core.Utility.Attributes
{
    public class NOValueMinAttribute : Attribute
    {
        public readonly float MinValue;
        
        public NOValueMinAttribute(float minValue)
        {
            this.MinValue = minValue;
        }
    }
}
