using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NOManagerState : NOScriptableObject
    {
        [ReadOnly, ShowInInspector, PropertyOrder(-1000)]
        public INOManager Instance;
    }
}