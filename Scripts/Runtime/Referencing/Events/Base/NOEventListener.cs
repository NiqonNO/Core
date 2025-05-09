using System;

namespace NiqonNO.Core
{
    public class NOEventListener<T> : INOEventListener<T>
    {
        private Action<T> Response;

        public NOEventListener(Action<T> response)
        {
            Response = response;
        }
        
        public void OnEventRaised(T item)
        {
            Response.Invoke(item);
        }
    }
}