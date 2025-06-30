using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace NiqonNO.Core
{
    public class NODataCollectionWrapper : SerializedMonoBehaviour
    {
        [OdinSerialize] 
        private INODataCollection DataCollection;
        public int Count => DataCollection.Count;

        public INODataProvider GetDataAt(int index) => DataCollection.GetGenericDataAt(index);
    }
}