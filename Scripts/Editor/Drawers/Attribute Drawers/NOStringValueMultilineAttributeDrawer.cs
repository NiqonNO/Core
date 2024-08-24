using NiqonNO.Core.Editor.PropertyDrawers;
using NiqonNO.Core.Utility.Attributes;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace NiqonNO.Core.Editor.AttributeDrawers
{
    public class NOStringValueMultilineAttributeDrawer<T1, T2> : OdinAttributeDrawer<NOStringValueMultilineAttribute, T1> where T1 : NOValue<string, T2> where T2 : NOValueAsset<string>
    {
        InspectorProperty UseReferenceProperty;
        InspectorProperty LocalValueProperty;
        
        private Vector2 scrollPosition;

        protected override void Initialize()
        {
            UseReferenceProperty = Property.Children["UseReference"];
            LocalValueProperty = Property.Children["LocalValue"];
        }
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if((bool)UseReferenceProperty.ValueEntry.WeakSmartValue)
            {
                CallNextDrawer(label);
                return;
            }

            SirenixEditorGUI.BeginVerticalPropertyLayout(GUIContent.none);
            if(label != null)
            {
                EditorGUILayout.LabelField(label);
            }
            
            SirenixEditorGUI.BeginHorizontalPropertyLayout(GUIContent.none);
            bool popupResult = EditorGUILayout.Popup((bool)UseReferenceProperty.ValueEntry.WeakSmartValue ? 0 : 1, 
                NOValueDrawer<T1,string,T2>.popupOptions, NOValueDrawer<T1,string,T2>.popupStyle, GUILayout.Width(10), GUILayout.MinHeight(15)) == 0;
            UseReferenceProperty.ValueEntry.WeakSmartValue = popupResult;
            
            IPropertyValueEntry<string> valueEntry = (IPropertyValueEntry<string>)LocalValueProperty.ValueEntry;
            float height = 32f + ((Mathf.Clamp(Mathf.CeilToInt(EditorStyles.textArea.CalcHeight(GUIHelper.TempContent(valueEntry.SmartValue), GUIHelper.ContextWidth) / 13f), Attribute.minLines, Attribute.maxLines) - 1) * 13);
            Rect controlRect = EditorGUILayout.GetControlRect(label != null, height);
            
            if (Event.current.type == EventType.Layout)
                GUIUtility.GetControlID(NOValueDrawer<T1,string,T2>.EditorGUI_s_TextAreaHash, FocusType.Keyboard, controlRect);
            LocalValueProperty.ValueEntry.WeakSmartValue = NOValueDrawer<T1,string,T2>.EditorGUI_ScrollableTextAreaInternal(controlRect, valueEntry.SmartValue, ref scrollPosition, EditorStyles.textArea);
            
            SirenixEditorGUI.EndHorizontalPropertyLayout();
            SirenixEditorGUI.EndVerticalPropertyLayout();
        }
    }
}