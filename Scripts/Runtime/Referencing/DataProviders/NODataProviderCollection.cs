using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NODataProviderCollection : NOScriptableObject
    {
        [SerializeField] 
        private List<NODataProvider> ItemData = default;
        public int Count => ItemData.Count;
        
        protected T GetDataAt<T>(int index) where T : NODataProvider
        {
            return ItemData[index] as T;
        }
        public NODataProvider GetDataAt(int index) => GetDataAt<NODataProvider>(index);
    }
}
