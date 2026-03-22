using System;
using System.Collections.Generic;
using System.Linq;
using NiqonNO.Core.Utility;
using Sirenix.Utilities;
using UnityEngine;

namespace NiqonNO.Core.Scene
{
    [Serializable]
    public class NOSceneDependencyData
    {
        private const string ResourcesScenePath = "Core";

        private static Dictionary<string, string> _PersistentSceneDependency;
        public static Dictionary<string, string> PersistentSceneDependency
        {
            get
            {
                if (_PersistentSceneDependency != null) return _PersistentSceneDependency;

                _PersistentSceneDependency = new();
                LoadSceneDependency();
                return _PersistentSceneDependency;
            }
        }

        private static void LoadSceneDependency()
        {
            var sceneDependencyTree = Resources.LoadAll<NOSceneDependencyTree>(ResourcesScenePath);
            if (sceneDependencyTree.IsNullOrEmpty())
            {
                Debug.LogError($"Could not find object of type {nameof(NOSceneDependencyTree)} in Resources \"{ResourcesScenePath}\" folder. Scene Dependency will not be loaded.");
                return;
            }
            if (sceneDependencyTree.Length > 1)
            {
                Debug.LogWarning($"More than one objects of type {nameof(NOSceneDependencyTree)} have been found in Resources \"{ResourcesScenePath}\" folder. First result will be used.");
            }
            sceneDependencyTree[0].SetToStaticContext();
        }
        public static void SetToStaticContext(List<NOSceneDependencyTree.SceneDependencyPair> dependencyTree)
        {
            PersistentSceneDependency.Clear();
            foreach (var pair in dependencyTree)
            {
                PersistentSceneDependency[pair.SceneName] = pair.SceneParent;
            }
        }

        public static void SetFromStaticContext(ref List<NOSceneDependencyTree.SceneDependencyPair> dependencyTree)
        {
            dependencyTree.Clear();
            foreach (var item in PersistentSceneDependency)
            {
                dependencyTree.Add(new NOSceneDependencyTree.SceneDependencyPair(item.Key, item.Value));
            }
        }
        
        public static List<string> GetSceneDependencies(string scene, bool rootAtTop)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(scene)) return result;

            var visited = new HashSet<string>();
            Visit(scene, visited, result);
            if (rootAtTop)
                result.Reverse();
            
            return result;
        }
        public static List<string> GetSceneDependencies(string scene, ICollection<string> existingCollection, bool rootAtTop)
        {
            var result = GetSceneDependencies(scene, rootAtTop);
            var unique = new HashSet<string>(result);

            foreach (var s in existingCollection)
            {
                if (string.IsNullOrWhiteSpace(s) || !unique.Add(s)) continue;
                result.Add(s);
            }

            return result;
        }
        private static void Visit(string scene, HashSet<string> visited, List<string> result)
        {
            if (string.IsNullOrWhiteSpace(scene) || !visited.Add(scene)) return;
            if (TryGetSceneParent(scene, out var dependency) && !string.IsNullOrWhiteSpace(dependency))
            {
                Visit(dependency, visited, result);
            }
            result.Add(scene);
        }

        public static bool TryGetSceneParent(string scene, out string parent)
        {
            if (!string.IsNullOrWhiteSpace(scene)) return PersistentSceneDependency.TryGetValue(scene, out parent);
            parent = string.Empty;
            return false;
        }
        
#if UNITY_EDITOR
        public static void UpdateSceneList()
        {
            var newDictionary = NOSceneUtility.GetScenesInBuildSettings().ToDictionary(
                scene => scene,
                scene => PersistentSceneDependency.TryGetValue(scene, value: out var value)
                    ? value : String.Empty);
            _PersistentSceneDependency = newDictionary;
        }
#endif

        public static bool SceneDependsOn(string item, string dependency)
        {
            if (string.IsNullOrWhiteSpace(item) || string.IsNullOrWhiteSpace(dependency))
                return false;

            var visited = new HashSet<string>();
            var current = item;

            while (!string.IsNullOrWhiteSpace(current) && visited.Add(current))
            {
                if (!PersistentSceneDependency.TryGetValue(current, out var parent) || string.IsNullOrWhiteSpace(parent))
                    return false;

                if (parent == dependency)
                    return true;

                current = parent;
            }

            return false;
        }
    }
}