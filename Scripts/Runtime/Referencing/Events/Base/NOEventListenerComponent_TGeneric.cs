using UnityEngine;
using UnityEngine.Events;

namespace NiqonNO.Core
{
    public abstract class NOEventListenerComponent<T1> : NOMonoBehaviour, INOEventListener<T1>
    {
        [SerializeField]
        private NOEventAsset<T1> EventAsset;
        [SerializeField] 
        private UnityEvent<T1> UnityResponseEvent;

        private void OnEnable()
        {
            if (EventAsset == null) return;
            EventAsset.RegisterListener(this);
        }
        
        private void OnDisable()
        {
            if (EventAsset == null) return;
            EventAsset.UnregisterListener(this);
        }
        
        public void OnEventRaised(T1 item)
        {
            UnityResponseEvent?.Invoke(item);
        }
    }
}