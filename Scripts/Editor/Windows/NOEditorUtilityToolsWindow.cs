using System;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;

namespace NiqonNO.Core.Editor
{
    public class NOEditorUtilityToolsWindow : OdinMenuEditorWindow
    {
        private NOEditorUtilityToolTab CurrentTab;
        
        [MenuItem("Tools/Niqon/Utility Tools")]
        private static void OpenWindow()
        {
            GetWindow(typeof(NOEditorUtilityToolsWindow)).Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tabs = TypeCache.GetTypesDerivedFrom<NOEditorUtilityToolTab>()
                .Where(type => !type.IsAbstract && !type.ContainsGenericParameters && type.GetConstructor(Type.EmptyTypes) != null)
                .Select(type => (NOEditorUtilityToolTab)Activator.CreateInstance(type))
                .OrderByDescending(tab => tab.Order)
                .ThenBy(tab => tab.TabName)
                .ToList();
            
            OdinMenuTree tree = new OdinMenuTree(false);
            tree.AddRange(tabs, tab => tab.TabName);
            tree.SortMenuItemsByName();
            
            tree.Selection.SelectionChanged += SelectionChanged;
            return tree;
        }

        private void SelectionChanged(SelectionChangedType obj)
        {
            var tab = MenuTree.Selection.Select(i => i.Value).FilterCast<NOEditorUtilityToolTab>().FirstOrDefault();
            if (tab == null) return;
            CurrentTab = tab;
        }

        protected override void DrawEditor(int index) => CurrentTab?.DrawGUI();
        
        protected override void OnDisable()
        {
            base.OnDisable();
            ClearCurrentTab();
        }

        protected override void OnDestroy() => ClearCurrentTab();

        void ClearCurrentTab()
        {
            CurrentTab = null;
        }
    }
}
