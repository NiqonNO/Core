using System;
using NiqonNO.Core.MVVM;
using NiqonNO.Core.Utility.Attributes;

namespace NiqonNO.Core.UI
{
    public abstract class NOSelectorCellViewModel<TData, TContext> : NOSelectorCell, INOMVVMViewModel<TData, TContext> where TData : INODataProvider where TContext : NOSelector
    {
        public Action OnViewModelChangedEvent { get; set; }
        
        [NOMVVMBind] 
        protected bool Selected => Index == Context.SelectedIndex;
        
        public TData ItemData { get; set; }
        public bool IsModelSet => ItemData != null;
        
        public TContext Context { get; set; }
        public bool IsContextSet => Context != null;
        
        protected override INODataProvider Data
        {
            get => ItemData;
            set => SeCellData(value);
        }
        
        public override void Initialize(NOSelector owner) => SetContext(owner as TContext);
        
        public override void SeCellData(INODataProvider itemData)
        {
            if (itemData is TData provider)
                SetData(provider);
        }
        public void SetData(TData itemData)
        {
            if (itemData.Equals(ItemData)) return;
            ItemData = itemData;
            OnViewModelChange();
        }
        public void SetContext(TContext context)
        {
            if (context.Equals(Context)) return;
            Context = context;
            OnViewModelChange();
        }
        
        public override void ForceRefresh() => OnViewModelChange();
        public void OnViewModelChange()
        {
            if (!IsModelSet) return;
            OnViewModelChangedEvent?.Invoke();
        }
    }
}