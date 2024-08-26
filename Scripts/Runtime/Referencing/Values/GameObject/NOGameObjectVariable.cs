using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOGameObjectVariable : NOVariable<GameObject>
    {
        public NOGameObjectVariable() : base(default) { }
        public NOGameObjectVariable(GameObject value) : base(value) { }
    }
}
