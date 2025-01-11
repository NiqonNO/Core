using NiqonNO.Core.Utility.Attributes;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.AttributeDrawers
{
    public class NOValueRangeAttributeDecimalDrawer<T1, T2> : OdinAttributeDrawer<NOValueRangeAttribute, T1> where T1 : NOValueBase<decimal, T2> where T2 : NOValueAsset<decimal>
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
            else NOEditorDrawerUtility.DrawSlider((IPropertyValueEntry<decimal>)LocalValueProperty.ValueEntry, Attribute.MinValue, Attribute.MaxValue);

            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
    }
}
