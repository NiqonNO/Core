using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NODataCollection<T> : NOEventAsset<T>, INODataCollection<T> where T : INODataProvider
    {
        [SerializeField] 
        private List<T> ItemData = default;
        List<T> INODataCollection<T>.ItemData => ItemData;
        
        public int Count => ItemData.Count;
    }
}