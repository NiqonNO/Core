using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NODataProviderCollection : NOEventAsset<NODataProvider>
    {
        public abstract int Count { get;  }
        public abstract NODataProvider GetGenericDataAt(int index);
    }
    
    public class NODataProviderCollection<T> : NODataProviderCollection where T : NODataProvider
    {
        [SerializeField] 
        private List<T> ItemData = default;
        public override int Count => ItemData.Count;

        
        public T GetDataAt(int index) =>  ItemData[index];
        public override NODataProvider GetGenericDataAt(int index) => GetDataAt(index);
    }
}