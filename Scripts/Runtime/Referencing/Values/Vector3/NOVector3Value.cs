using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOVector3Value : NOValue<Vector3>
    {
        public NOVector3Value() : base() { }
        public NOVector3Value(Vector3 value) : base(value) { }
        public NOVector3Value(NOVector3Reference value) : base(value) { }
    }
}
