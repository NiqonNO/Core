using UnityEngine;

namespace NiqonNO.Core
{
    public interface INOEventListener<T>
    {
        void OnEventRaised(T item);
    }
}
