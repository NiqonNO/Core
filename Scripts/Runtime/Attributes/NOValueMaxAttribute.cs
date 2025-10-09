using System;

namespace NiqonNO.Core
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
