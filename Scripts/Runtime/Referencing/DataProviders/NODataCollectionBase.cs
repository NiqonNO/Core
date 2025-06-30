using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
    public abstract class NODataCollectionBase : NOEventAsset<NODataProvider>
    {
        public abstract int Count { get;  }
        public abstract NODataProvider GetGenericDataAt(int index);
    }
}