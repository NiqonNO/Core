using System;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOStringVariable : NOVariable<string>
    {
        public NOStringVariable() : base(default) { }
        public NOStringVariable(string value) : base(value) { }
    }
}
