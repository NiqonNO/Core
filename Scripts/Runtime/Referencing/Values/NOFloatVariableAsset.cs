using System;

namespace NiqonNO.Core
{
    [Serializable] public class NOFloatVariableAsset : NOVariableAsset<float> {}

    [Serializable]
    public class NOFloatVariable : NOVariable<float>
    {
        public NOFloatVariable() : base(default) { }
        public NOFloatVariable(float value) : base(value) { }
    }
}
