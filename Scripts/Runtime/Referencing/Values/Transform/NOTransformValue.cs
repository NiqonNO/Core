using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOTransformValue : NOValue<Transform>
    {
        public NOTransformValue() : base(default) { }
        public NOTransformValue(Transform value) : base(value) { }
    }
}
