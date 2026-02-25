using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace NiqonNO.Core
{
    public abstract class NOEventAsset<T> : NOScriptableObject
    {
        private readonly HashSet<INOEventListener<T>> EventListeners = new();

        [Button]
        public void Raise(T item)
        {
            foreach (var listener in EventListeners)
            {
                listener.OnEventRaised(item);
            }
        }

        public void RegisterListener(INOEventListener<T> listener)
        {
            EventListeners.Add(listener);
        }
        public void UnregisterListener(INOEventListener<T> listener)
        {
            EventListeners.Remove(listener);
        }
    }
}