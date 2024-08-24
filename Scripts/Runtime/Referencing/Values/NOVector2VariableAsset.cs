using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public class NOVector2VariableAsset : NOVariableAsset<Vector2> {}
    [Serializable] public class NOVector2Variable : NOVariable<Vector2, NOVariableAsset<Vector2>>
    {
        public NOVector2Variable(Vector2 value) : base(value)
        {
        }
    }
}
