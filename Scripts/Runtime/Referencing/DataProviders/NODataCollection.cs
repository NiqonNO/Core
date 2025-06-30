using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NODataCollection<T> : NODataCollectionBase where T : NODataProvider
    {
        [SerializeField] 
        private List<T> ItemData = default;
        public override int Count => ItemData.Count;

        
        public T GetDataAt(int index) =>  ItemData[index];
        public override NODataProvider GetGenericDataAt(int index) => GetDataAt(index);
    }
}