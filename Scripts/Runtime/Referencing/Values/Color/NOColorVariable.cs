using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOColorVariable : NOVariable<Color>
    {
        public NOColorVariable() : base(default) { }
        public NOColorVariable(Color value) : base(value) { }
    }
}