using System;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOCharValue : NOValue<char>
    {
        public NOCharValue() : base() { }
        public NOCharValue(char value) : base(value) { }
        public NOCharValue(NOCharReference value) : base(value) { }
    }
}
