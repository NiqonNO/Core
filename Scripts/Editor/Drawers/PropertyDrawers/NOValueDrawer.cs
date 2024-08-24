using System;
using System.Reflection;
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
        
        internal static readonly ScrollableTextAreaInternalDelegate EditorGUI_ScrollableTextAreaInternal;
        internal static readonly FieldInfo EditorGUI_s_TextAreaHash_Field;
        internal static readonly int EditorGUI_s_TextAreaHash;
        internal static readonly string[] popupOptions = { "Use Reference", "Use Variable" };
        internal static readonly GUIStyle popupStyle;
        
        static NOValueDrawer()
        {
            MethodInfo method = typeof (EditorGUI).GetMethod("ScrollableTextAreaInternal", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
                EditorGUI_ScrollableTextAreaInternal = (ScrollableTextAreaInternalDelegate) Delegate.CreateDelegate(typeof (ScrollableTextAreaInternalDelegate), method);
            EditorGUI_s_TextAreaHash_Field = typeof (EditorGUI).GetField("s_TextAreaHash", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (!(EditorGUI_s_TextAreaHash_Field != null))
                return;
            EditorGUI_s_TextAreaHash = (int) EditorGUI_s_TextAreaHash_Field.GetValue(null);
            popupStyle = new GUIStyle(GUI.skin.GetStyle("StaticDropdown"))
                { imagePosition = ImagePosition.ImageOnly};
        }

        protected override void Initialize()
        {
            UseReferenceProperty = Property.Children["UseReference"];
            LocalValueProperty = Property.Children["LocalValue"];
            LocalReferenceProperty = Property.Children["LocalReference"];
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
        
        internal delegate string ScrollableTextAreaInternalDelegate(
            Rect position,
            string text,
            ref Vector2 scrollPosition,
            GUIStyle style);
    }
}