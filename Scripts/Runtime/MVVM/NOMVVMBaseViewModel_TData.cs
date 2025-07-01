using System;
using UnityEngine;

namespace NiqonNO.Core.MVVM
{
    public abstract class NOMVVBaseViewModel<TData> : MonoBehaviour, INOMVVMViewModel<TData>
    {
        public TData ItemData { get; set; }
        public bool IsModelSet => ItemData != null;
        public Action OnViewModelChangedEvent { get; set; }
        
        public void SetData(TData itemData)
        {
            if (itemData.Equals(ItemData)) return;
            ItemData = itemData;
            OnViewModelChange();
        }

        public void OnViewModelChange()
        {
            if (!IsModelSet) return;
            OnViewModelChangedEvent?.Invoke();
        }

    }
}