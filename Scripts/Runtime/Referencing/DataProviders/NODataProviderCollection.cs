using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NODataProviderCollection<T> : NOEventAsset<T> where T : NODataProvider
    {
        [SerializeField] 
        private List<T> ItemData = default;
        public int Count => ItemData.Count;
        
        public T GetDataAt(int index)
        {
            return ItemData[index];
        }
    }
}