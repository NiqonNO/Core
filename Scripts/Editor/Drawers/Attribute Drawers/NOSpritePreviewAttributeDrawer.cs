using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.AttributeDrawers
{
    public class NOSpritePreviewAttributeDrawer : OdinAttributeDrawer<NOSpritePreviewAttribute, Sprite>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
            
            var sprite = ValueEntry.SmartValue;
            if (sprite == null) return;

            var tex = sprite.texture;
            var r = sprite.textureRect;
            var uv = new Rect(
                r.x / tex.width,
                r.y / tex.height,
                r.width / tex.width,
                r.height / tex.height
            );
            
            GUILayout.BeginHorizontal(GUI.skin.box);
            Rect rect = GUILayoutUtility.GetAspectRect(r.width / r.height);
            GUI.DrawTextureWithTexCoords(rect, tex, uv, true);
            GUILayout.EndHorizontal();
        }
    }
}