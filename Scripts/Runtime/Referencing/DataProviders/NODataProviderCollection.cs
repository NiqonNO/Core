using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NODataProviderCollection : NOScriptableObject
    {
        [SerializeField] 
        List<NODataProvider> ItemData = default;

        public int Count => ItemData.Count;
        public NODataProvider GetDataAt(int index) => ItemData[index];
    }
}
