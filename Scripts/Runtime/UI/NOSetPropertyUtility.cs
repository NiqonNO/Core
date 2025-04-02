using System;
using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core.UI
{
    internal static class NOSetPropertyUtility
    {
        public static void SetColor(ref Color currentValue, Color newValue, Action onChangeCallback)
        {
            if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b &&
                currentValue.a == newValue.a)
                return;

            currentValue = newValue;
            onChangeCallback.Invoke();
        }

        public static void SetStruct<T>(ref T currentValue, T newValue, Action onChangeCallback) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return;

            currentValue = newValue;
            onChangeCallback.Invoke();
        }

        public static void SetClass<T>(ref T currentValue, T newValue, Action onChangeCallback) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;

            currentValue = newValue;
            onChangeCallback.Invoke();
        }
    }
}
