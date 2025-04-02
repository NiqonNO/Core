using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.UI;

namespace NiqonNO.Core.UI.Editor
{
    [CustomEditor(typeof(NOSlider), true)]
    public class NOSliderEditor : SelectableEditor
    {
        private PropertyTree propertyTree;

        protected override void OnEnable()
        {
            base.OnEnable();
            propertyTree = PropertyTree.Create(serializedObject.targetObject);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (propertyTree == null) return;
            
            propertyTree.BeginDraw(true);
            foreach (var inspectorProperty in propertyTree.EnumerateTree())
            {
                if (typeof(NOSlider).IsAssignableFrom(inspectorProperty.Info.GetMemberInfo()?.DeclaringType))
                {
                    inspectorProperty.Draw();
                }
            }
            propertyTree.EndDraw();
        }

        protected override void OnDisable()
        {
            propertyTree?.Dispose();
            base.OnDisable();
        }
    }
}