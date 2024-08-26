using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public class NOTransformValueAsset : NOValueAsset<Transform> {}
    [Serializable] public class NOTransformValue : NOValue<Transform, NOValueAsset<Transform>>
    {
        //public NOTransformValue() : base(default) { }
        public NOTransformValue(Transform value) : base(value) { }
    }
}
