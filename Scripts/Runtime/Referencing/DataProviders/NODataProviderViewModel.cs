using NiqonNO.Core.MVVM;
using NiqonNO.Core.Utility.Attributes;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NODataProviderViewModel<T> : NOMVVMBaseViewModel<NODataProvider, T> where T : class
    {
        [NOMVVMBind] 
        protected string ItemName => ItemData.ItemName;
        
        [NOMVVMBind] 
        protected Sprite ItemIcon => ItemData.ItemIcon;
        
        [NOMVVMBind] 
        protected Color ItemColor => ItemData.ItemColor;
    }
    
    public class NODataProviderViewModel : NOMVVMBaseViewModel<NODataProvider>
    {
        [NOMVVMBind] 
        protected string ItemName => ItemData.ItemName;
        
        [NOMVVMBind] 
        protected Sprite ItemIcon => ItemData.ItemIcon;
        
        [NOMVVMBind] 
        protected Color ItemColor => ItemData.ItemColor;
    }
}
