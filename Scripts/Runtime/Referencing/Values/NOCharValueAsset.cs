using System;

namespace NiqonNO.Core
{
    [Serializable] public class NOCharValueAsset : NOValueAsset<char> {}
    [Serializable] public class NOCharValue : NOValue<char, NOValueAsset<char>>
    {
        public NOCharValue(char value) : base(value)
        {
        }
    }
}
