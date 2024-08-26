using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public abstract class NOValueBase<T1, T2>  where T2 : NOValueAsset<T1>
    {
        [SerializeField] 
        protected bool UseReference;
        [SerializeField, HideLabel, InlineProperty] 
        protected T1 LocalValue;
        [SerializeField, HideLabel, InlineEditor] 
        protected T2 LocalReference;

        protected NOValueBase(T1 value)
        {
            LocalValue = value;
        }
    }
}