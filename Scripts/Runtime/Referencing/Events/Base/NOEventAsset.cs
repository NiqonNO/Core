using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace NiqonNO.Core
{
    public class NOEventAsset : NOScriptableObject
    {
        private readonly HashSet<INOEventListener> EventListeners = new();

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
            EventListeners.Add(listener);
        }
        public void UnregisterListener(INOEventListener listener)
        {
            EventListeners.Remove(listener);
        }
    }
}
