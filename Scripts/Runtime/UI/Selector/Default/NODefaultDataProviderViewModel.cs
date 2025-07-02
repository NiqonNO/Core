using System;
using NiqonNO.Core.MVVM;
using NiqonNO.Core.Utility.Attributes;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NODefaultDataProviderViewModel : NOMVVBaseViewModel<NODefaultDataProvider>
    {
        [NOMVVMBind] 
        protected string ItemName => ItemData.ItemName;
        
        [NOMVVMBind] 
        protected Sprite ItemIcon => ItemData.ItemIcon;
        
        [NOMVVMBind] 
        protected Color ItemColor => ItemData.ItemColor;

        public void SetData(INODataProvider itemData)
        {
            if (itemData is NODefaultDataProvider provider)
                base.SetData(provider);
        }
    }
}
