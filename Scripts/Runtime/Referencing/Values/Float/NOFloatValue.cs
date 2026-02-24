using System;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOFloatValue : NOValue<float>
    {
        public NOFloatValue() : base() { }
        public NOFloatValue(float value) : base(value) { }
        public NOFloatValue(NOFloatReference value) : base(value) { }
    }
}
