using UnityEngine;
using UnityEngine.Events;

namespace NiqonNO.Core
{
    public abstract class NOEventListenerComponent<T1, T2> : NOMonoBehaviour, INOEventListener where T1 : NOEventAsset where T2 : UnityEvent
    {
        [SerializeField]
        private T1 EventAsset;
        [SerializeField] 
        private T2 UnityResponseEvent;
        
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
        
        public void OnEventRaised()
        {
            UnityResponseEvent?.Invoke();
        }
    }
    
        public abstract class NOEventListenerComponent<T1, T2, T3> : NOMonoBehaviour, INOEventListener<T1> where T2 : NOEventAsset<T1> where T3 : UnityEvent<T1>
    {
        [SerializeField]
        private T2 EventAsset;
        [SerializeField] 
        private T3 UnityResponseEvent;

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
