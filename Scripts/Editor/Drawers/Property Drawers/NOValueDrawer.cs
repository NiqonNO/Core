using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.PropertyDrawers
{
    public class NOValueDrawer<T1, T2, T3> : OdinValueDrawer<T1> where T1 : NOValueBase<T2, T3> where T3 : NOValueAsset<T2>
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
            SirenixEditorGUI.BeginHorizontalPropertyLayout(label ?? GUIContent.none);
            
            bool popupResult = NOEditorDrawerUtility.DrawReferenceDropDown((bool)UseReferenceProperty.ValueEntry.WeakSmartValue);
            UseReferenceProperty.ValueEntry.WeakSmartValue = popupResult;

            if (popupResult) LocalReferenceProperty.Draw(GUIContent.none);
            else LocalValueProperty.Draw(GUIContent.none);
            
            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
    }
}