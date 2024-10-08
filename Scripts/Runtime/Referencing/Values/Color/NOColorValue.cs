using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOColorValue : NOValue<Color>
    {
        public NOColorValue() : base(default) { }
        public NOColorValue(Color value) : base(value) { }
    }
}