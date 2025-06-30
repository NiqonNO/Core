using UnityEngine;

namespace NiqonNO.Core
{
    public interface INOEventListener
    {
        void OnEventRaised();
    }
    
    public interface INOEventListener<T>
    {
        void OnEventRaised(T item);
    }
}
