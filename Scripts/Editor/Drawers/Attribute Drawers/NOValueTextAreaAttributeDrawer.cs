using NiqonNO.Core.Utility.Attributes;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.AttributeDrawers
{
    public class NOValueTextAreaAttributeDrawer<T1, T2> : OdinAttributeDrawer<NOValueTextAreaAttribute, T1> where T1 : NOValueBase<string, T2> where T2 : NOValueAsset<string>
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
            bool popupResult = (bool)UseReferenceProperty.ValueEntry.WeakSmartValue;
            if (popupResult)
            {
                SirenixEditorGUI.BeginHorizontalPropertyLayout(label ?? GUIContent.none);
                UseReferenceProperty.ValueEntry.WeakSmartValue = NOEditorDrawerUtility.DrawReferenceDropDown((bool)UseReferenceProperty.ValueEntry.WeakSmartValue);
                LocalReferenceProperty.Draw(GUIContent.none);
            }
            else
            {
                EditorGUILayout.LabelField(label);
                SirenixEditorGUI.BeginHorizontalPropertyLayout(GUIContent.none);
                UseReferenceProperty.ValueEntry.WeakSmartValue = NOEditorDrawerUtility.DrawReferenceDropDown((bool)UseReferenceProperty.ValueEntry.WeakSmartValue);
                NOEditorDrawerUtility.DrawTextArea((IPropertyValueEntry<string>)LocalValueProperty.ValueEntry, ref ScrollPosition, Attribute.MinLines, Attribute.MaxLines);
            }
            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
    }
}