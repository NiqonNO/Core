using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public class NOVector3VariableAsset : NOVariableAsset<Vector3> {}

    [Serializable]
    public class NOVector3Variable : NOVariable<Vector3>
    {
        public NOVector3Variable() : base(default) { }
        public NOVector3Variable(Vector3 value) : base(value) { }
    }
}
