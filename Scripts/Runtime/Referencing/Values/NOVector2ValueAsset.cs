using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public class NOVector2ValueAsset : NOValueAsset<Vector2> {}
    [Serializable] public class NOVector2Value : NOValue<Vector2, NOValueAsset<Vector2>>
    {
        public NOVector2Value(Vector2 value) : base(value)
        {
        }
    }
}
