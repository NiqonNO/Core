using System;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOIntValue : NOValue<int>
    {
        public NOIntValue() : base(default) { }
        public NOIntValue(int value) : base(value) { }
    }
}
