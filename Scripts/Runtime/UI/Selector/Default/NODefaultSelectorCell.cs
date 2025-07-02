using System;
using NiqonNO.Core.MVVM;
using NiqonNO.Core.Utility.Attributes;
using UnityEngine;

namespace NiqonNO.Core.UI
{
    public class NODefaultSelectorCell : NOSelectorCellViewModel<NODefaultDataProvider, NOSelector>
    {
        [NOMVVMBind] 
        protected string ItemName => ItemData.ItemName;
        
        [NOMVVMBind] 
        protected Sprite ItemIcon => ItemData.ItemIcon;
        
        [NOMVVMBind] 
        protected Color ItemColor => ItemData.ItemColor;
        
        [NOMVVMBind] 
        protected string ItemIndex => $"{Index + 1}/{Context.TotalCount}";
        
        [NOMVVMBind] 
        protected bool Selected => Index == Context.SelectedIndex;
        
        public override bool IsVisible() => gameObject.activeSelf;
        public override void SetVisible(bool visible) => gameObject.SetActive(visible);

        public override void ForceRefresh() => OnViewModelChange();
    }
}