using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOTransformVariable : NOVariable<Transform>
    {
        public NOTransformVariable() : base(default) { }
        public NOTransformVariable(Transform value) : base(value) { }
    }
}
