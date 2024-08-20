using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace NiqonNO.Core
{
    public abstract class NOEventAsset : NOScriptableObject
    {
        private readonly List<INOEventListener> EventListeners = new();

        [Button]
        public void Raise()
        {
            foreach (var listener in EventListeners)
            {
                listener.OnEventRaised();
            }
        }

        public void RegisterListener(INOEventListener listener)
        {
            if (EventListeners.Contains(listener)) return;
            EventListeners.Add(listener);
        }
        public void UnregisterListener(INOEventListener listener)
        {
            if (!EventListeners.Contains(listener)) return;
            EventListeners.Remove(listener);
        }
    }
}
