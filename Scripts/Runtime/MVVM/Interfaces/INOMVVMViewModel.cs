using System;

namespace NiqonNO.Core.MVVM
{
    public interface INOMVVMViewModel
    {
        Action OnViewModelChangedEvent { get; set; }
        bool IsModelSet { get; }

        public void OnViewModelChange();

        public void RegisterOnViewModelChangeEvent(Action bind)
        {
            OnViewModelChangedEvent += bind;
            if(IsModelSet) bind.Invoke();
        }
        public void UnregisterOnViewModelChangeEvent(Action bind)
        {
            OnViewModelChangedEvent -= bind;
        }
    }
}
