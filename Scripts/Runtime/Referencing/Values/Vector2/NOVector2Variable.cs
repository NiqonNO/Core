using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOVector2Variable : NOVariable<Vector2>
    {
        public NOVector2Variable() : base(default) { }
        public NOVector2Variable(Vector2 value) : base(value) { }
    }
}
