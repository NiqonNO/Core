using NiqonNO.Core.Utility.Attributes;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.AttributeDrawers
{
    public class NOSpritePreviewAttributeDrawer : OdinAttributeDrawer<NOSpritePreviewAttribute, Sprite>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
            
            if (ValueEntry.SmartValue == null) return;

            GUILayout.BeginHorizontal(GUI.skin.box);
            Rect rect = GUILayoutUtility.GetAspectRect((float)ValueEntry.SmartValue.texture.width / ValueEntry.SmartValue.texture.height);
            GUI.DrawTexture(rect, ValueEntry.SmartValue.texture, ScaleMode.ScaleToFit);
            GUILayout.EndHorizontal();
        }
    }
}