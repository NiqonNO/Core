using System;
using NiqonNO.Core.Utility.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NODataCollectionWrapper : SerializedMonoBehaviour
    {
        [SerializeField, NORequireInterface(typeof(INODataCollection))]
        private UnityEngine.Object _DataCollection;
        private INODataCollection DataCollection => _DataCollection as INODataCollection;
        public int Count => DataCollection.Count;

        public INODataProvider GetDataAt(int index) => DataCollection.GetGenericDataAt(index);
    }
}