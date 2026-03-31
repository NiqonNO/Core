using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace NiqonNO.Core.Editor
{
    public sealed class NOPrefabReparenterToolTab : NOEditorUtilityToolTab
    {
        [ShowInInspector, AssetsOnly] private GameObject TargetParent;
        [ShowInInspector] public List<GameObject> SourceObjects = new();

        public override string TabName => "Prefab Reparenter";
        public override int Order => 1;

        [Button]
        void Execute()
        {
            if (TargetParent == null)
            {
                Debug.LogError("[Prefab Reparenter] TargetParent is null.");
                return;
            }

            if (!PrefabUtility.IsPartOfPrefabAsset(TargetParent))
            {
                Debug.LogError("[Prefab Reparenter] TargetParent must be a prefab asset.");
                return;
            }

            if (SourceObjects == null || SourceObjects.Count == 0)
                return;

            var targetParentPath = AssetDatabase.GetAssetPath(TargetParent);
            if (string.IsNullOrEmpty(targetParentPath))
            {
                Debug.LogError("[Prefab Reparenter] Could not resolve TargetParent asset path.");
                return;
            }

            foreach (var source in SourceObjects)
            {
                if (source == null)
                    continue;

                try
                {
                    ReparentOne(source, targetParentPath);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Prefab Reparenter] Failed for '{source.name}': {ex}");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void ReparentOne(GameObject source, string targetParentPath)
        {
            var sourcePath = AssetDatabase.GetAssetPath(source);
            var outputPath = string.IsNullOrEmpty(sourcePath)
                ? AssetDatabase.GenerateUniqueAssetPath(BuildFallbackPath(source))
                : sourcePath;

            GameObject sourceCopy = null;
            GameObject variantRoot = null;
            bool unloadPrefabContents = false;

            try
            {
                sourceCopy = CreateWorkingCopy(source, out unloadPrefabContents);

                if (sourceCopy == null)
                    throw new InvalidOperationException($"Could not create a working copy for '{source.name}'.");

                TryUnpackVariant(source, sourceCopy);

                variantRoot = (GameObject)PrefabUtility.InstantiatePrefab(TargetParent);
                if (variantRoot == null)
                    throw new InvalidOperationException($"Could not instantiate TargetParent for '{source.name}'.");

                variantRoot.name = source.name;

                SynchronizeGameObject(sourceCopy, variantRoot);

                bool success;
                PrefabUtility.SaveAsPrefabAssetAndConnect(
                    variantRoot,
                    outputPath,
                    InteractionMode.AutomatedAction,
                    out success);

                if (!success)
                    throw new InvalidOperationException($"Saving prefab failed for '{source.name}' -> '{outputPath}'.");
            }
            finally
            {
                if (variantRoot != null)
                    UnityEngine.Object.DestroyImmediate(variantRoot);

                if (sourceCopy != null)
                {
                    if (unloadPrefabContents)
                        PrefabUtility.UnloadPrefabContents(sourceCopy);
                    else
                        UnityEngine.Object.DestroyImmediate(sourceCopy);
                }
            }
        }

        private static GameObject CreateWorkingCopy(GameObject source, out bool unloadPrefabContents)
        {
            unloadPrefabContents = false;

            var sourcePath = AssetDatabase.GetAssetPath(source);
            if (!string.IsNullOrEmpty(sourcePath))
            {
                unloadPrefabContents = true;
                return PrefabUtility.LoadPrefabContents(sourcePath);
            }

            return UnityEngine.Object.Instantiate(source);
        }

        private static void TryUnpackVariant(GameObject originalSource, GameObject workingCopy)
        {
            if (originalSource == null || workingCopy == null)
                return;

            if (!PrefabUtility.IsPartOfVariantPrefab(originalSource))
                return;

            if (!PrefabUtility.IsAnyPrefabInstanceRoot(workingCopy))
                return;

            PrefabUtility.UnpackPrefabInstance(
                workingCopy,
                PrefabUnpackMode.Completely,
                InteractionMode.AutomatedAction);
        }

        private static void SynchronizeGameObject(GameObject source, GameObject target)
        {
            if (source == null || target == null)
                return;

            target.name = source.name;
            target.tag = source.tag;
            target.layer = source.layer;
            target.isStatic = source.isStatic;
            target.SetActive(source.activeSelf);

            SynchronizeChildren(source.transform, target.transform);
            SynchronizeComponents(source, target);
            CopyTransformData(source.transform, target.transform);
        }

        private static void SynchronizeComponents(GameObject source, GameObject target)
        {
            var sourceComponents = source.GetComponents<Component>();
            var targetComponents = target.GetComponents<Component>();

            var targetBuckets = BuildComponentBuckets(targetComponents);
            var matched = new HashSet<Component>();

            foreach (var sourceComponent in sourceComponents)
            {
                if (sourceComponent == null)
                    continue;

                if (sourceComponent is Transform)
                    continue;

                var type = sourceComponent.GetType();

                Component targetComponent = null;
                if (targetBuckets.TryGetValue(type, out var queue) && queue.Count > 0)
                {
                    targetComponent = queue.Dequeue();
                    matched.Add(targetComponent);
                }
                else
                {
                    try
                    {
                        targetComponent = target.AddComponent(type);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning(
                            $"[Prefab Reparenter] Could not add component '{type.Name}' to '{target.name}': {ex.Message}");
                        continue;
                    }
                }

                if (targetComponent == null)
                    continue;

                try
                {
                    EditorUtility.CopySerializedIfDifferent(sourceComponent, targetComponent);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(
                        $"[Prefab Reparenter] Could not copy component '{type.Name}' on '{target.name}': {ex.Message}");
                }
            }

            foreach (var targetComponent in targetComponents)
            {
                if (targetComponent == null)
                    continue;

                if (targetComponent is Transform)
                    continue;

                if (matched.Contains(targetComponent))
                    continue;

                try
                {
                    UnityEngine.Object.DestroyImmediate(targetComponent);
                }
                catch
                {
                    // Best effort.
                }
            }
        }

        private static void SynchronizeChildren(Transform sourceParent, Transform targetParent)
        {
            var sourceChildren = GetChildren(sourceParent);
            var targetChildren = GetChildren(targetParent);

            var targetBuckets = BuildChildBuckets(targetChildren);
            var matched = new HashSet<Transform>();

            for (int i = 0; i < sourceChildren.Count; i++)
            {
                var sourceChild = sourceChildren[i];
                var sourceChildType = sourceChild.GetType();

                Transform targetChild = null;

                if (targetBuckets.TryGetValue(sourceChild.name, out var queue) && queue.Count > 0)
                {
                    var candidate = queue.Peek();
                    if (CanReuseChild(sourceChildType, candidate))
                    {
                        targetChild = queue.Dequeue();
                        matched.Add(targetChild);
                    }
                }

                if (targetChild == null)
                {
                    var childGo = sourceChild is RectTransform
                        ? new GameObject(sourceChild.name, typeof(RectTransform))
                        : new GameObject(sourceChild.name);

                    targetChild = childGo.transform;
                    targetChild.SetParent(targetParent, false);
                }

                targetChild.SetSiblingIndex(i);
                SynchronizeGameObject(sourceChild.gameObject, targetChild.gameObject);
                matched.Add(targetChild);
            }

            foreach (var targetChild in targetChildren)
            {
                if (targetChild == null)
                    continue;

                if (matched.Contains(targetChild))
                    continue;

                try
                {
                    UnityEngine.Object.DestroyImmediate(targetChild.gameObject);
                }
                catch
                {
                    // Best effort.
                }
            }
        }

        private static void CopyTransformData(Transform source, Transform target)
        {
            if (source == null || target == null)
                return;

            if (source.GetType() == target.GetType())
            {
                EditorUtility.CopySerializedIfDifferent(source, target);
                return;
            }

            target.localPosition = source.localPosition;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }

        private static bool CanReuseChild(Type sourceChildType, Transform targetChild)
        {
            if (sourceChildType == null || targetChild == null)
                return false;

            if (sourceChildType == targetChild.GetType())
                return true;

            if (sourceChildType == typeof(Transform) && targetChild is RectTransform)
                return true;

            return false;
        }

        private static Dictionary<Type, Queue<Component>> BuildComponentBuckets(Component[] components)
        {
            var buckets = new Dictionary<Type, Queue<Component>>();

            foreach (var component in components)
            {
                if (component == null)
                    continue;

                if (component is Transform)
                    continue;

                var type = component.GetType();
                if (!buckets.TryGetValue(type, out var queue))
                {
                    queue = new Queue<Component>();
                    buckets.Add(type, queue);
                }

                queue.Enqueue(component);
            }

            return buckets;
        }

        private static Dictionary<string, Queue<Transform>> BuildChildBuckets(List<Transform> children)
        {
            var buckets = new Dictionary<string, Queue<Transform>>();

            foreach (var child in children)
            {
                if (child == null)
                    continue;

                if (!buckets.TryGetValue(child.name, out var queue))
                {
                    queue = new Queue<Transform>();
                    buckets.Add(child.name, queue);
                }

                queue.Enqueue(child);
            }

            return buckets;
        }

        private static List<Transform> GetChildren(Transform parent)
        {
            var result = new List<Transform>(parent.childCount);
            for (int i = 0; i < parent.childCount; i++)
                result.Add(parent.GetChild(i));
            return result;
        }

        private string BuildFallbackPath(GameObject source)
        {
            var targetParentPath = AssetDatabase.GetAssetPath(TargetParent);
            var folder = "Assets";

            if (!string.IsNullOrEmpty(targetParentPath))
            {
                folder = Path.GetDirectoryName(targetParentPath)?.Replace('\\', '/') ?? "Assets";
                if (string.IsNullOrEmpty(folder))
                    folder = "Assets";
            }

            return $"{folder}/{source.name}.prefab";
        }
    }
}