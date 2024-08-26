using System;

namespace NiqonNO.Core
{
    [Serializable] public class NOCharVariableAsset : NOVariableAsset<char> {}

    [Serializable]
    public class NOCharVariable : NOVariable<char>
    {
        public NOCharVariable() : base(default) { }
        public NOCharVariable(char value) : base(value) { }
    }
}
