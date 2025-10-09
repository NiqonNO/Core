using System;

namespace NiqonNO.Core
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
