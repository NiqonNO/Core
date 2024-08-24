using System;

namespace NiqonNO.Core
{
    [Serializable] public class NOBoolVariableAsset : NOVariableAsset<bool> {}
    [Serializable] public class NOBoolVariable : NOVariable<bool, NOVariableAsset<bool>>
    {
        public NOBoolVariable(bool value) : base(value)
        {
        }
    }
}
