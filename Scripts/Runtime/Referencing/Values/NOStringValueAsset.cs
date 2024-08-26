using System;

namespace NiqonNO.Core
{
    [Serializable] public class NOStringValueAsset : NOValueAsset<string> {}
    [Serializable] public class NOStringValue : NOValue<string, NOValueAsset<string>>
    {
        //public NOStringValue() : base(default) { }
        public NOStringValue(string value) : base(value) { }
    }
}
