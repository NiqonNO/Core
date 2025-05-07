using UnityEngine;

namespace NiqonNO.Core
{
    public class NODataProviderCollectionViewModel : NODataProviderViewModel
    {
        [SerializeField] 
        private NODataProviderCollection ItemCollection;

        public void SetData(int index) => SetData(ItemCollection.GetDataAt(index));
    }
}
