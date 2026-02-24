using System;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOStringValue : NOValue<string>
    {
        public NOStringValue() : base() { }
        public NOStringValue(string value) : base(value) { }
        public NOStringValue(NOStringReference value) : base(value) { }
    }
}
