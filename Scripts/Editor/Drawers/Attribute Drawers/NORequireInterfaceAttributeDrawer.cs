using NiqonNO.Core.Utility.Attributes;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.AttributeDrawers
{
    public class NORequireInterfaceAttributeDrawer<T> : OdinAttributeDrawer<NORequireInterfaceAttribute, T> where T : Object
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var requiredType = this.Attribute.requiredType;

            if(ValueEntry.SmartValue != null && !requiredType.IsAssignableFrom(typeof(T)))
            {
                SirenixEditorGUI.ErrorMessageBox($"Property is not a {requiredType.Name} reference type.");
            }

            ValueEntry.SmartValue = SirenixEditorFields.UnityObjectField(label, ValueEntry.SmartValue, requiredType, true) as T;
        }
    }
}