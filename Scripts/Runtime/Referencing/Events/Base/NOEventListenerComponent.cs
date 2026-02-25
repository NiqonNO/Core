using UnityEngine;
using UnityEngine.Events;

namespace NiqonNO.Core
{
    public class NOEventListenerComponent : NOMonoBehaviour, INOEventListener
    {
        [SerializeField]
        private NOEventAsset EventAsset;
        [SerializeField] 
        private UnityEvent UnityResponseEvent;
        
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
}
