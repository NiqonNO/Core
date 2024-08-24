using System;

namespace NiqonNO.Core
{
    [Serializable] public class NOIntValueAsset : NOValueAsset<int> {}
    [Serializable] public class NOIntValue : NOValue<int, NOValueAsset<int>>
    {
        public NOIntValue(int value) : base(value)
        {
        }
    }
}
