using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.AttributeDrawers
{
    public class NOValueRangeAttributeSByteDrawer<T1> : OdinAttributeDrawer<NOValueRangeAttribute, T1> where T1 : NOValue<sbyte>
    {
        InspectorProperty UseReferenceProperty;
        InspectorProperty LocalValueProperty;
        InspectorProperty LocalReferenceProperty;
        
        protected override void Initialize()
        {
            UseReferenceProperty = Property.Children["UseReference"];
            LocalValueProperty = Property.Children["LocalValue"];
            LocalReferenceProperty = Property.Children["LocalReference"];
        }
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if((bool)UseReferenceProperty.ValueEntry.WeakSmartValue
               && LocalReferenceProperty.ValueEntry.WeakSmartValue == null)
                SirenixEditorGUI.ErrorMessageBox("LocalReference should not be null when UseReference is set to true. Value will return default.");
            
            SirenixEditorGUI.BeginHorizontalPropertyLayout(label ?? GUIContent.none);
            
            bool popupResult = NOEditorDrawerUtility.DrawReferenceDropDown((bool)UseReferenceProperty.ValueEntry.WeakSmartValue);
            UseReferenceProperty.ValueEntry.WeakSmartValue = popupResult;

            if (popupResult) LocalReferenceProperty.Draw(GUIContent.none);
            else NOEditorDrawerUtility.DrawSlider((IPropertyValueEntry<sbyte>)LocalValueProperty.ValueEntry, Attribute.MinValue, Attribute.MaxValue);

            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
    }
}
