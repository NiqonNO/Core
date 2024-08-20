using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace NiqonNO.Core.Editor.PropertyDrawers
{
    public class NOValueDrawer<T1, T2, T3> : OdinValueDrawer<T1> where T1 : NOValue<T2, T3> where T3 : NOValueAsset<T2>
    {
        InspectorProperty UseReferenceProperty;
        InspectorProperty LocalValueProperty;
        InspectorProperty LocalReferenceProperty;
        
        private readonly string[] popupOptions = { "Use Reference", "Use Variable" };
        private GUIStyle popupStyle;

        protected override void Initialize()
        {
            UseReferenceProperty = Property.Children["UseReference"];
            LocalValueProperty = Property.Children["LocalValue"];
            LocalReferenceProperty = Property.Children["LocalReference"];
            popupStyle ??= new GUIStyle(GUI.skin.GetStyle("StaticDropdown"))
                { imagePosition = ImagePosition.ImageOnly};
        }
        protected override void DrawPropertyLayout(GUIContent label)
        {
            SirenixEditorGUI.BeginHorizontalPropertyLayout(label ?? GUIContent.none);
            
            bool popupResult = EditorGUILayout.Popup((bool)UseReferenceProperty.ValueEntry.WeakSmartValue ? 0 : 1, 
                popupOptions, popupStyle, GUILayout.Width(10), GUILayout.MinHeight(15)) == 0;
            UseReferenceProperty.ValueEntry.WeakSmartValue = popupResult;

            if (popupResult) LocalReferenceProperty.Draw(GUIContent.none);
            else LocalValueProperty.Draw(GUIContent.none);
            
            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
    }
}