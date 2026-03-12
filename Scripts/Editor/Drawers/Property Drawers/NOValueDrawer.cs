using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.PropertyDrawers
{
    public class NOValueDrawer<T1, T2> : OdinValueDrawer<T1> where T1 : NOValue<T2>
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
            if (UseReferenceProperty == null) return;
            if (LocalValueProperty == null) return;
            if (LocalReferenceProperty == null) return;
            if((bool)UseReferenceProperty.ValueEntry.WeakSmartValue
               && LocalReferenceProperty.ValueEntry.WeakSmartValue == null)
            SirenixEditorGUI.ErrorMessageBox("LocalReference should not be null when UseReference is set to true. Value will return default.");
            
            SirenixEditorGUI.BeginHorizontalPropertyLayout(label ?? GUIContent.none);
            
            bool popupResult = NOEditorDrawerUtility.DrawReferenceDropDown((bool)UseReferenceProperty.ValueEntry.WeakSmartValue);
            UseReferenceProperty.ValueEntry.WeakSmartValue = popupResult;

            if (popupResult) LocalReferenceProperty.Draw(GUIContent.none);
            else LocalValueProperty.Draw(GUIContent.none);
            
            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
    }
}