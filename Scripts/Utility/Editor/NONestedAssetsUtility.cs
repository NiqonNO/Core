using System.Linq;
using UnityEngine;

namespace NiqonNO.Core.Utility.Editor
{
    public static class NONestedAssetsUtility
    {
        public static void CreateNestedItem<T>(ScriptableObject parentAsset, T item, bool overrideExisting = false) where T : Object
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(parentAsset);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Asset path is invalid.");
                return;
            }

            UnityEditor.AssetDatabase.StartAssetEditing();

            var assets = UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
            if (overrideExisting)
            {
                foreach (var child in assets)
                {
                    if (child.name == item.name && child.GetType() == item.GetType())
                    {
                        UnityEditor.AssetDatabase.RemoveObjectFromAsset(child);
                        Object.DestroyImmediate(child);
                    }
                }
            }
            else
            {
                item.name = NONamingUtility.EnsureUniqueName(assets, item);
            }

            UnityEditor.AssetDatabase.AddObjectToAsset(item, parentAsset);

            UnityEditor.AssetDatabase.StopAssetEditing();
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}