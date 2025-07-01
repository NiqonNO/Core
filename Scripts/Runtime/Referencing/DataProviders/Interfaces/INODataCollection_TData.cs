using System.Collections.Generic;

namespace NiqonNO.Core
{
    public interface INODataCollection<T> : INODataCollection where T : INODataProvider
    {
        List<T> ItemData { get; }
        T GetDataAt(int index) => ItemData[index];
        INODataProvider INODataCollection.GetGenericDataAt(int index) => GetDataAt(index);
    }
}