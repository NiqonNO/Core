using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public class NOGameObjectVariableAsset : NOVariableAsset<GameObject> {}

    [Serializable]
    public class NOGameObjectVariable : NOVariable<GameObject>
    {
        public NOGameObjectVariable() : base(default) { }
        public NOGameObjectVariable(GameObject value) : base(value) { }
    }
}
