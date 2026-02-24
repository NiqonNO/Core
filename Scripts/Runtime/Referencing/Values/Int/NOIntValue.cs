using System;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOIntValue : NOValue<int>
    {
        public NOIntValue() : base() { }
        public NOIntValue(int value) : base(value) { }
        public NOIntValue(NOIntReference value) : base(value) { }
    }
}
