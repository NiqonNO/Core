using System;
using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOWorldObject : NOMonoBehaviour
    {
        private void Awake()
        {
        }

        public abstract void Initialize();
    }
}