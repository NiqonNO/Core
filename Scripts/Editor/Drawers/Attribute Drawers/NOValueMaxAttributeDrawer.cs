using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.AttributeDrawers
{
    public class NOValueMaxAttributeDrawer<T, T1, T2> : OdinAttributeDrawer<NOValueMaxAttribute, T1> where T1 : NOValueBase<T, T2> where T2 : NOValueAsset<T> where T : struct
    {
        InspectorProperty UseReferenceProperty;
        InspectorProperty LocalValueProperty;
        InspectorProperty LocalReferenceProperty;
        
        private static readonly bool IsNumber = GenericNumberUtility.IsNumber(typeof (T));
        private static readonly bool IsVector = GenericNumberUtility.IsVector(typeof (T));
        
        public override bool CanDrawTypeFilter(System.Type type) => IsNumber || IsVector;
        
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
            else
            {
                LocalValueProperty.Draw(GUIContent.none);
                IPropertyValueEntry smartValue = LocalValueProperty.ValueEntry;
                if (GenericNumberUtility.NumberIsInRange((object) smartValue, double.MinValue, Attribute.MaxValue))
                    return;
                LocalValueProperty.ValueEntry.WeakSmartValue = GenericNumberUtility.Clamp<T>((T)smartValue.WeakSmartValue, double.MinValue, Attribute.MaxValue);
            }

            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
    }
}
