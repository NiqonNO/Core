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
        
        internal static void DrawTextArea(IPropertyValueEntry<string> valueEntry, ref Vector2 scrollPosition, int minLines = 3, int maxLines = 3)
        {
            float height = 32f + (Mathf.Clamp(Mathf.CeilToInt(EditorStyles.textArea.CalcHeight(GUIHelper.TempContent(valueEntry.SmartValue), GUIHelper.ContextWidth) / 13f), minLines, maxLines) - 1) * 13;
            Rect controlRect = EditorGUILayout.GetControlRect(true, height);
            if (EditorGUI_ScrollableTextAreaInternal == null || EditorGUI_s_TextAreaHash_Field == null)
            {
                EditorGUI.LabelField(controlRect, GUIHelper.TempContent("Cannot draw TextArea because Unity's internal API has changed."));
                return;
            }

            if (Event.current.type == EventType.Layout)
                GUIUtility.GetControlID(EditorGUI_s_TextAreaHash, FocusType.Keyboard, controlRect);
            valueEntry.SmartValue = EditorGUI_ScrollableTextAreaInternal(controlRect, valueEntry.SmartValue, ref scrollPosition, EditorStyles.textArea);
        }

        internal static void DrawSlider(IPropertyValueEntry<short> valueEntry, float minValue, float maxValue)
        {
            EditorGUI.BeginChangeCheck();
            int num = SirenixEditorFields.RangeIntField((int) valueEntry.SmartValue, Math.Max((int) short.MinValue, (int) minValue), Math.Min((int) short.MaxValue, (int) maxValue));
            if (!EditorGUI.EndChangeCheck())
                return;
            if (num < (int) short.MinValue)
                num = (int) short.MinValue;
            else if (num > (int) short.MaxValue)
                num = (int) short.MaxValue;
            valueEntry.SmartValue = (short) num;
        }
        internal static void DrawSlider(IPropertyValueEntry<int> valueEntry, float minValue, float maxValue)
        {
            EditorGUI.BeginChangeCheck();
            int num = SirenixEditorFields.RangeIntField(valueEntry.SmartValue, (int) minValue, (int) maxValue);
            if (!EditorGUI.EndChangeCheck())
                return;
            valueEntry.SmartValue = num;
        }
        internal static void DrawSlider(IPropertyValueEntry<long> valueEntry, float minValue, float maxValue)
        {
            long num1 = valueEntry.SmartValue;
            if (num1 < (long) int.MinValue)
                num1 = (long) int.MinValue;
            else if (num1 > (long) int.MaxValue)
                num1 = (long) int.MaxValue;
            EditorGUI.BeginChangeCheck();
            int num2 = SirenixEditorFields.RangeIntField((int) num1, (int) minValue, (int) maxValue);
            if (!EditorGUI.EndChangeCheck())
                return;
            valueEntry.SmartValue = (long) num2;
        }
        
        internal static void DrawSlider(IPropertyValueEntry<ushort> valueEntry, float minValue, float maxValue)
        {
            EditorGUI.BeginChangeCheck();
            int num = SirenixEditorFields.RangeIntField((int) valueEntry.SmartValue, Math.Max(0, (int) minValue), Math.Min((int) ushort.MaxValue, (int) maxValue));
            if (!EditorGUI.EndChangeCheck())
                return;
            if (num < 0)
                num = 0;
            else if (num > (int) ushort.MaxValue)
                num = (int) ushort.MaxValue;
            valueEntry.SmartValue = (ushort) num;
        }
        internal static void DrawSlider(IPropertyValueEntry<uint> valueEntry, float minValue, float maxValue)
        {
            uint num1 = valueEntry.SmartValue;
            if (num1 > (uint) int.MaxValue)
                num1 = (uint) int.MaxValue;
            EditorGUI.BeginChangeCheck();
            int num2 = SirenixEditorFields.RangeIntField((int) num1, Math.Max(0, (int) minValue), (int) maxValue);
            if (!EditorGUI.EndChangeCheck())
                return;
            if (num2 < 0)
                num2 = 0;
            valueEntry.SmartValue = (uint) num2;
        }
        internal static void DrawSlider(IPropertyValueEntry<ulong> valueEntry, float minValue, float maxValue)
        {
            ulong num1 = valueEntry.SmartValue;
            if (num1 > (ulong) int.MaxValue)
                num1 = (ulong) int.MaxValue;
            EditorGUI.BeginChangeCheck();
            int num2 = SirenixEditorFields.RangeIntField((int) num1, Math.Max(0, (int) minValue), (int) maxValue);
            if (!EditorGUI.EndChangeCheck())
                return;
            if (num2 < 0)
                num2 = 0;
            valueEntry.SmartValue = (ulong) num2;
        }
        
        internal static void DrawSlider(IPropertyValueEntry<byte> valueEntry, float minValue, float maxValue)
        {
            EditorGUI.BeginChangeCheck();
            int num = SirenixEditorFields.RangeIntField((int) valueEntry.SmartValue, Math.Max(0, (int) minValue), Math.Min((int) byte.MaxValue, (int) maxValue));
            if (!EditorGUI.EndChangeCheck())
                return;
            if (num < 0)
                num = 0;
            else if (num > (int) byte.MaxValue)
                num = (int) byte.MaxValue;
            valueEntry.SmartValue = (byte) num;
        }
        internal static void DrawSlider(IPropertyValueEntry<sbyte> valueEntry, float minValue, float maxValue)
        {
            EditorGUI.BeginChangeCheck();
            int num = SirenixEditorFields.RangeIntField((int) valueEntry.SmartValue, Math.Max((int) sbyte.MinValue, (int) minValue), Math.Min((int) sbyte.MaxValue, (int) maxValue));
            if (!EditorGUI.EndChangeCheck())
                return;
            if (num < (int) sbyte.MinValue)
                num = (int) sbyte.MinValue;
            else if (num > (int) sbyte.MaxValue)
                num = (int) sbyte.MaxValue;
            valueEntry.SmartValue = (sbyte) num;
        }
        
        internal static void DrawSlider(IPropertyValueEntry<float> valueEntry, float minValue, float maxValue)
        {
            EditorGUI.BeginChangeCheck();
            float num = SirenixEditorFields.RangeFloatField(valueEntry.SmartValue, minValue, maxValue);
            if (!EditorGUI.EndChangeCheck())
                return;
            valueEntry.SmartValue = num;
        }
        internal static void DrawSlider(IPropertyValueEntry<double> valueEntry, float minValue, float maxValue)
        {
            double num1 = valueEntry.SmartValue;
            if (num1 < -3.4028234663852886E+38)
                num1 = -3.4028234663852886E+38;
            else if (num1 > 3.4028234663852886E+38)
                num1 = 3.4028234663852886E+38;
            EditorGUI.BeginChangeCheck();
            double num2 = (double) SirenixEditorFields.RangeFloatField((float) num1, minValue, maxValue);
            if (!EditorGUI.EndChangeCheck())
                return;
            valueEntry.SmartValue = num2;
        }
        internal static void DrawSlider(IPropertyValueEntry<decimal> valueEntry, float minValue, float maxValue)
        {
            EditorGUI.BeginChangeCheck();
            float num = SirenixEditorFields.RangeFloatField((float) valueEntry.SmartValue, minValue, maxValue);
            if (!EditorGUI.EndChangeCheck())
                return;
            valueEntry.SmartValue = (decimal) num;
        }
        
        private delegate string ScrollableTextAreaInternalDelegate(
            Rect position,
            string text,
            ref Vector2 scrollPosition,
            GUIStyle style);
    }
}