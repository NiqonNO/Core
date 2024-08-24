using System;

namespace NiqonNO.Core
{
    [Serializable] public class NOFloatVariableAsset : NOVariableAsset<float> {}
    [Serializable] public class NOFloatVariable : NOVariable<float, NOVariableAsset<float>>
    {
        public NOFloatVariable(float value) : base(value)
        {
        }
    }
}
