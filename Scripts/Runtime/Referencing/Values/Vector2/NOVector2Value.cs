using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOVector2Value : NOValue<Vector2>
    {
        public NOVector2Value() : base() { }
        public NOVector2Value(Vector2 value) : base(value) { }
        public NOVector2Value(NOVector2Reference value) : base(value) { }
    }
}
