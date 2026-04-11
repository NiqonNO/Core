using UnityEditor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers
{
	public abstract class NOShaderGUI : ShaderGUI
	{
		protected MaterialEditor MaterialEditor { get; private set; }
		protected MaterialProperty[] Properties { get; private set; }

		public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			MaterialEditor = materialEditor;
			Properties = properties;

			FindProperties();
			DrawProperties();

			foreach (var target in materialEditor.targets)
			{
				SetKeywords(target as Material);
			}
		}

		protected abstract void FindProperties();
		protected abstract void DrawProperties();
		protected abstract void SetKeywords(Material target);

		protected void TexturePropertySingleLine(MaterialProperty textureProp, GUIContent textureLabel,
			MaterialProperty extraProperty1, MaterialProperty extraProperty2, MaterialProperty extraProperty3)
		{
			if (textureProp == null) return;

			Rect rectForSingleLine = EditorGUILayout.GetControlRect(true, 20f, EditorStyles.layerMaskField);

			MaterialEditor.TexturePropertyMiniThumbnail(rectForSingleLine, textureProp, textureLabel.text,
				textureLabel.tooltip);

			int indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			rectForSingleLine = MaterialEditor.GetRectAfterLabelWidth(rectForSingleLine);
			float propWidth = rectForSingleLine.width / 3f;
			rectForSingleLine.width = propWidth;
			MaterialEditor.ShaderProperty(rectForSingleLine, extraProperty1, string.Empty);
			rectForSingleLine.x += propWidth;
			MaterialEditor.ShaderProperty(rectForSingleLine, extraProperty2, string.Empty);
			rectForSingleLine.x += propWidth;
			MaterialEditor.ShaderProperty(rectForSingleLine, extraProperty3, string.Empty);
			EditorGUI.indentLevel = indentLevel;
		}

		protected void MinMaxSliderWithFields(GUIContent label, ref float center, ref float edge, int minLimit, int maxLimit)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(label, GUILayout.Width(EditorGUIUtility.labelWidth));
			center = EditorGUILayout.DelayedFloatField(center, GUILayout.Width(EditorGUIUtility.fieldWidth));
			EditorGUILayout.MinMaxSlider(ref center, ref edge, minLimit, maxLimit);
			edge = EditorGUILayout.DelayedFloatField(edge, GUILayout.Width(EditorGUIUtility.fieldWidth));
			EditorGUILayout.EndHorizontal();
		}
		
		protected void DrawMetallic(MaterialProperty metallicMap, GUIContent metallicLabel, MaterialProperty metallic,
			MaterialProperty smoothness, GUIContent smoothnessLabel)
		{
			if (metallicMap == null) return;

			MaterialEditor.TexturePropertySingleLine(metallicLabel, metallicMap,
				metallicMap.textureValue ? null : metallic);

			if (smoothness == null) return;
			EditorGUI.indentLevel += 2;
			MaterialEditor.ShaderProperty(smoothness, smoothnessLabel);
			EditorGUI.indentLevel -= 2;
		}

		protected void DrawNormal(MaterialProperty normalMap, GUIContent normalLabel, MaterialProperty normalScale)
		{
			if (normalMap == null) return;

			MaterialEditor.TexturePropertySingleLine(normalLabel, normalMap,
				normalMap.textureValue ? normalScale : null);
		}

		protected MaterialProperty FindProperty(string name) => FindProperty(name, Properties);

		protected void SetKeyword(Material target, string keyword, bool state)
		{
			if (state)
				target.EnableKeyword(keyword);
			else
				target.DisableKeyword(keyword);
		}
	}
}