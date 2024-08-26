using System;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOStringValue : NOValue<string>
    {
        public NOStringValue() : base(default) { }
        public NOStringValue(string value) : base(value) { }
    }
}
