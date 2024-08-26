using System;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOFloatValue : NOValue<float>
    {
        public NOFloatValue() : base(default) { }
        public NOFloatValue(float value) : base(value) { }
    }
}
