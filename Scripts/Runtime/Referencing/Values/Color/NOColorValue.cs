using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public class NOColorValue : NOValue<Color>
    {
        public NOColorValue() : base() { }
        public NOColorValue(Color value) : base(value) { }
        public NOColorValue(NOColorReference value) : base(value) { }
    }
}