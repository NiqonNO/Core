using System;

namespace NiqonNO.Core
{
    public class NOEventListener<T> : INOEventListener<T>
    {
        private event Action<T> Response;

        public NOEventListener() { }
        public NOEventListener(Action<T> response)
        {
            Response = response;
        }

        public void AddResponse(Action<T> response)
        {
            Response += response;
        }
        
        public void RemoveResponse(Action<T> response)
        {
            Response -= response;
        }
        
        public void ClearResponse()
        {
            Response = null;
        }
        
        public void OnEventRaised(T item)
        {
            Response?.Invoke(item);
        }
    }
}