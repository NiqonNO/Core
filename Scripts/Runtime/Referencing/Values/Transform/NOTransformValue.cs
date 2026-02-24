using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOTransformValue : NOValue<Transform>
    {
        public NOTransformValue() : base() { }
        public NOTransformValue(Transform value) : base(value) { }
        public NOTransformValue(NOTransformReference value) : base(value) { }
    }
}
