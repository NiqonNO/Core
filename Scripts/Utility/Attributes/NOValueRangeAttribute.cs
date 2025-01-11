using System;

namespace NiqonNO.Core.Utility.Attributes
{
    public class NOValueRangeAttribute : Attribute
    {
        public readonly float MinValue;
        public readonly float MaxValue;
        
        public NOValueRangeAttribute(float minValue, float maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }
    }
}
