using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NiqonNO.Core.Editor
{
    public class NOcriptableObjectCreator : OdinMenuEditorWindow
    {
        static HashSet<Type> scriptableObjectTypes = AssemblyUtilities.GetTypes(AssemblyCategory.Scripts)
            .Where(t =>
                t.IsClass &&
                typeof(NOScriptableObject).IsAssignableFrom(t) &&
                !typeof(NOManagerState).IsAssignableFrom(t) &&
                !typeof(EditorWindow).IsAssignableFrom(t) &&
                !typeof(UnityEditor.Editor).IsAssignableFrom(t))
           .ToHashSet();

        [MenuItem("Assets/Create Scriptable Object", priority = -1000)]
        private static void ShowDialog()
        {
            var path = "Assets";
            var obj = Selection.activeObject;
            if (obj && AssetDatabase.Contains(obj))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!Directory.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }
            }

            var window = CreateInstance<NOcriptableObjectCreator>();
            window.ShowUtility();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
            window.titleContent = new GUIContent(path);
            window.targetFolder = path.Trim('/');
        }

        private ScriptableObject previewObject;
        private string targetFolder;
        private Vector2 scroll;

        private Type SelectedType
        {
            get
            {
                var m = this.MenuTree.Selection.LastOrDefault();
                return m == null ? null : m.Value as Type;
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            MenuWidth = 270;
            WindowPadding = Vector4.zero;

            OdinMenuTree tree = new OdinMenuTree(false)
            {
                Config =
                {
                    DrawSearchToolbar = true
                },
                DefaultMenuStyle = OdinMenuStyle.TreeViewStyle
            };
            tree.AddRange(scriptableObjectTypes.Where(x => !x.IsAbstract), GetMenuPathForType).AddThumbnailIcons();
            tree.SortMenuItemsByName();
            tree.Selection.SelectionConfirmed += x => CreateAsset();
            tree.Selection.SelectionChanged += e =>
            {
                if (previewObject && !AssetDatabase.Contains(previewObject))
                {
                    DestroyImmediate(previewObject);
                }

                if (e != SelectionChangedType.ItemAdded)
                {
                    return;
                }

                var t = SelectedType;
                if (t is { IsAbstract: false })
                {
                    previewObject = CreateInstance(t);
                }
            };

            return tree;
        }

        private string GetMenuPathForType(Type t) => t.GetDerivedPath(typeof(NOScriptableObject));

        protected override IEnumerable<object> GetTargets()
        {
            yield return this.previewObject;
        }

        protected override void DrawEditor(int index)
        {
            this.scroll = GUILayout.BeginScrollView(this.scroll);
            {
                base.DrawEditor(index);
            }
            GUILayout.EndScrollView();

            if (this.previewObject)
            {
                GUILayout.FlexibleSpace();
                SirenixEditorGUI.HorizontalLineSeparator(1);
                if (GUILayout.Button("Create Asset", GUILayoutOptions.Height(30)))
                {
                    this.CreateAsset();
                }
            }
        }

        private void CreateAsset()
        {
            if (this.previewObject)
            {
                var dest = this.targetFolder + "/new " + this.MenuTree.Selection.First().Name.ToLower() + ".asset";
                dest = AssetDatabase.GenerateUniqueAssetPath(dest);
                AssetDatabase.CreateAsset(this.previewObject, dest);
                AssetDatabase.Refresh();
                Selection.activeObject = this.previewObject;
                EditorApplication.delayCall += this.Close;
            }
        }
    }
}