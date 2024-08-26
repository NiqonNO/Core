using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOGameObjectValue : NOValue<GameObject>
    {
        public NOGameObjectValue() : base(default) { }
        public NOGameObjectValue(GameObject value) : base(value) { }
    }
}
