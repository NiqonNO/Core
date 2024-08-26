using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOVector3Value : NOValue<Vector3>
    {
        public NOVector3Value() : base(default) { }
        public NOVector3Value(Vector3 value) : base(value) { }
    }
}
