using System.Collections.Generic;

namespace NiqonNO.Core
{
    public interface INODataCollection
    {
        int Count { get; }
        INODataProvider GetGenericDataAt(int index);
    }
}