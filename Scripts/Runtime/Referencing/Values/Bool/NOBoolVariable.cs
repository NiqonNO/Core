using System;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOBoolVariable : NOVariable<bool>
    {
        public NOBoolVariable() : base(default) { }
        public NOBoolVariable(bool value) : base(value) { }
    }
}
