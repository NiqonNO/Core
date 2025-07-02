using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NODefaultDataProvider : NOScriptableObject, INODataProvider
    {
        [SerializeField] 
        private NOStringValue _ItemName;
        public string ItemName => _ItemName.Value;
        
        [SerializeField] 
        private Sprite _ItemIcon;
        public Sprite ItemIcon => _ItemIcon;
        
        [SerializeField] 
        private NOColorValue _ItemColor;
        public Color ItemColor => _ItemColor.Value;
    }
}
