using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.AttributeDrawers
{
    public class NOValidateAttributeDrawer : OdinAttributeDrawer<NOValidateAttribute>
    {
        private static readonly Color InvalidColor = new Color(1f, 0.2f, 0.2f, 0.35f);

        private Func<object, bool> Validator;

        protected override void Initialize()
        {
            var host = Property.Tree.WeakTargets[0];
            var hostType = host.GetType();

            var method = hostType.GetMethod(Attribute.MethodName, Flags.AllMembers);

            if (method == null)
            {
                Debug.LogError($"NOValidate: Method '{Attribute.MethodName}' not found on {hostType}");
                return;
            }

            this.Validator = Validator;
            bool Validator(object value)
            {
                if (value == null) return true;
                return (bool)method.Invoke(host, new[] { value });
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (Property is { ChildResolver: IOrderedCollectionResolver })
            {
                CallNextDrawer(label);
                return;
            }

            if (Property.Parent is { ChildResolver: IOrderedCollectionResolver })
            {
                DrawSingle(label);
                return;
            }
            GUILayout.BeginVertical();
            DrawSingle(label);
            GUILayout.EndVertical();

        }

        private void DrawSingle(GUIContent label)
        {
            var evt = Event.current.type;
            if (evt == EventType.Layout)
            {
                CallNextDrawer(label);
                return;
            }

            bool isValid = false;

            if (Validator != null && Property.ValueEntry != null)
            {
                var value = Property.ValueEntry.WeakSmartValue;
                isValid = Validator(value);
            }
            
            if (Event.current.type == EventType.Repaint && !isValid)
            {
                var rect = GUIHelper.GetCurrentLayoutRect();
                SirenixEditorGUI.DrawSolidRect(rect, InvalidColor);
            }
            
            CallNextDrawer(label);
        }
    }
}