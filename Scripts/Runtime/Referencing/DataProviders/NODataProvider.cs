using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NODataProvider : NOScriptableObject
    {
        [SerializeField, ShowIf(nameof(OverrideName))] 
        private NOStringValue ItemNameOverride;
        protected virtual bool OverrideName => true;
        public virtual string ItemName => ItemNameOverride.Value;
        
        [SerializeField, ShowIf(nameof(OverrideIcon))] 
        private Sprite ItemIconOverride;
        protected virtual bool OverrideIcon => true;
        public virtual Sprite ItemIcon => ItemIconOverride;
        
        [SerializeField, ShowIf(nameof(OverrideColor))] 
        private NOColorValue ItemColorOverride;
        protected virtual bool OverrideColor => true;
        public virtual Color ItemColor => ItemColorOverride.Value;
    }
}
