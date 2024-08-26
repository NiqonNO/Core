using System;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOIntVariable : NOVariable<int>
    {
        public NOIntVariable() : base(default) { }
        public NOIntVariable(int value) : base(value) { }
    }
}
