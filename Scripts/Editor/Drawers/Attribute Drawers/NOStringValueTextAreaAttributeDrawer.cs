using NiqonNO.Core.Utility.Attributes;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.AttributeDrawers
{
    public class NOStringValueTextAreaAttributeDrawer<T1, T2> : OdinAttributeDrawer<NOStringValueTextAreaAttribute, T1> where T1 : NOValueBase<string, T2> where T2 : NOValueAsset<string>
    {
        InspectorProperty UseReferenceProperty;
        InspectorProperty LocalValueProperty;
        InspectorProperty LocalReferenceProperty;
        
        private Vector2 ScrollPosition;
        
        protected override void Initialize()
        {
            UseReferenceProperty = Property.Children["UseReference"];
            LocalValueProperty = Property.Children["LocalValue"];
            LocalReferenceProperty = Property.Children["LocalReference"];
        }
        protected override void DrawPropertyLayout(GUIContent label)
        {
            SirenixEditorGUI.BeginHorizontalPropertyLayout((bool)UseReferenceProperty.ValueEntry.WeakSmartValue && label != null ? label : GUIContent.none);
            
            bool popupResult = NOEditorDrawerUtility.DrawReferenceDropDown((bool)UseReferenceProperty.ValueEntry.WeakSmartValue);
            UseReferenceProperty.ValueEntry.WeakSmartValue = popupResult;

            if (popupResult) LocalReferenceProperty.Draw(GUIContent.none);
            else LocalValueProperty.ValueEntry.WeakSmartValue = NOEditorDrawerUtility.DrawTextArea(label, (IPropertyValueEntry<string>)LocalValueProperty.ValueEntry, ref ScrollPosition, Attribute.minLines, Attribute.maxLines);

            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
    }
}