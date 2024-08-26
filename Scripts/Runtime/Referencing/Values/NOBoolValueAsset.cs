using System;

namespace NiqonNO.Core
{
    [Serializable] public class NOBoolValueAsset : NOValueAsset<bool> { }

    [Serializable]
    public class NOBoolValue : NOValue<bool>
    {
        public NOBoolValue() : base(default) { }
        public NOBoolValue(bool value) : base(value) { }
    }
}
