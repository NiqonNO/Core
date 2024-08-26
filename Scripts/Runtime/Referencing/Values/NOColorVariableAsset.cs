using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public class NOColorVariableAsset : NOVariableAsset<Color> {}
    [Serializable] public class NOColorVariable : NOVariable<Color, NOVariableAsset<Color>>
    {
        public NOColorVariable() : base(default) { }
        public NOColorVariable(Color value) : base(value) { }
    }
}