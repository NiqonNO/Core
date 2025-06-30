using NiqonNO.Core.MVVM;
using NiqonNO.Core.Utility.Attributes;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NODataCollectionViewModel : NOMVVMBaseViewModel<NODataProvider>
    {
        [NOMVVMBind] 
        protected string ItemName => ItemData.ItemName;
        
        [NOMVVMBind] 
        protected Sprite ItemIcon => ItemData.ItemIcon;
        
        [NOMVVMBind] 
        protected Color ItemColor => ItemData.ItemColor;
        
        [SerializeField] 
        private NODataCollectionBase ItemCollectionBase;

        public void SetData(int index) => SetData(ItemCollectionBase.GetGenericDataAt(index));
    }
}