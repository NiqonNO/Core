using System;
using UnityEngine;

namespace NiqonNO.Core.MVVM
{
    public abstract class NOMVVMBaseViewModel<TData> : MonoBehaviour, INOMVVMViewModel
    {
        private TData _ItemData;
        protected TData ItemData => _ItemData;
        
        public bool IsModelSet => ItemData != null;

        public Action OnViewModelChangedEvent { get; set; }
        
        public virtual void SetData(TData itemData)
        {
            if (itemData.Equals(_ItemData)) return;
            _ItemData = itemData;
            OnViewModelChange();
        }

        public void OnViewModelChange()
        {
            if (!IsModelSet) return;
            OnViewModelChangedEvent?.Invoke();
        }
    }
    
    public abstract class NOMVVMBaseViewModel<TData, TContext> : NOMVVMBaseViewModel<TData>  where TContext : class
    {
        private TContext _Context;
        protected TContext Context => _Context;
        
        public bool IsContextSet => Context != null;
        
        public virtual void SetContext(TContext context)
        {
            if (context.Equals(_Context)) return;
            _Context = context;
            OnViewModelChange();
        }
    }
}