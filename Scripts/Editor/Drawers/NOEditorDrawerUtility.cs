using System;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers
{
    internal static class NOEditorDrawerUtility
    {
        private static readonly ScrollableTextAreaInternalDelegate EditorGUI_ScrollableTextAreaInternal;
        private static readonly FieldInfo EditorGUI_s_TextAreaHash_Field;
        private static readonly int EditorGUI_s_TextAreaHash;
        private static readonly string[] popupOptions = { "Use Reference", "Use Variable" };
        private static readonly GUIStyle popupStyle;
        
        static NOEditorDrawerUtility()
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

        internal static bool DrawReferenceDropDown(bool state)
        {
            return EditorGUILayout.Popup(state ? 0 : 1, popupOptions, popupStyle, GUILayout.Width(10), GUILayout.MinHeight(15)) == 0;
        }
        
        internal static string DrawTextArea(GUIContent label, IPropertyValueEntry<string> valueEntry,
            ref Vector2 scrollPosition, int minLines = 3, int maxLines = 3)
        {
            float height = 32f + (Mathf.Clamp(Mathf.CeilToInt(EditorStyles.textArea.CalcHeight(GUIHelper.TempContent(valueEntry.SmartValue), GUIHelper.ContextWidth) / 13f), minLines, maxLines) - 1) * 13;
            Rect controlRect = EditorGUILayout.GetControlRect(label != null, height);
            if (EditorGUI_ScrollableTextAreaInternal == null || EditorGUI_s_TextAreaHash_Field == null)
            {
                EditorGUI.LabelField(controlRect, label,GUIHelper.TempContent("Cannot draw TextArea because Unity's internal API has changed."));
                return valueEntry.SmartValue;
            }

            if (label != null)
            {
                Rect rect = new Rect(controlRect) { height = 16f };
                controlRect.yMin += rect.height;
                GUIHelper.IndentRect(ref rect);
                EditorGUI.HandlePrefixLabel(controlRect, rect, label);
            }

            if (Event.current.type == EventType.Layout)
                GUIUtility.GetControlID(EditorGUI_s_TextAreaHash, FocusType.Keyboard, controlRect);
            return EditorGUI_ScrollableTextAreaInternal(controlRect, valueEntry.SmartValue, ref scrollPosition, EditorStyles.textArea);
        }
        
        private delegate string ScrollableTextAreaInternalDelegate(
            Rect position,
            string text,
            ref Vector2 scrollPosition,
            GUIStyle style);
    }
}