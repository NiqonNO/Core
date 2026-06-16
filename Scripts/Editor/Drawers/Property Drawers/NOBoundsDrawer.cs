using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.PropertyDrawers
{
	public class NOBoundsDrawer<T> : OdinValueDrawer<T>
	{
		protected override void DrawPropertyLayout(GUIContent label)
		{
			EditorGUI.BeginChangeCheck();

			CallNextDrawer(label);

			if (EditorGUI.EndChangeCheck())
			{
				SceneView.RepaintAll();
			}
		}
	}
	
	public class NOCylindricalBoundsDrawer : NOBoundsDrawer<NOCylindricalBounds> { }
	public class NOSphereBoundsDrawer : NOBoundsDrawer<NOSphereBounds> { }
}