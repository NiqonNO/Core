using System;

namespace NiqonNO.Core
{
    [Serializable] public class NOCharValueAsset : NOValueAsset<char> {}

    [Serializable]
    public class NOCharValue : NOValue<char>
    {
        public NOCharValue() : base(default) { }
        public NOCharValue(char value) : base(value) { }
    }
}
