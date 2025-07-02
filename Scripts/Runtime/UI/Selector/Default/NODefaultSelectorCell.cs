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
    }
}