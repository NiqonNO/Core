using System;
using System.Collections.Generic;
using System.Linq;
using NiqonNO.Core.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core.Scene
{
    [Serializable]
    public class NOSceneDependencyData : ISerializationCallbackReceiver
    {
        private static Dictionary<string, string> PersistentSceneDependency { get; set; } = new();

        [SerializeField, ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false),
         OnValueChanged(nameof(ValidateChange), true)]
        private List<SceneDependencyPair> DependencyTree = new();

        public static List<string> GetSceneDependencies(string scene, bool rootAtTop)
        {
            var result = new List<string>();
            var visited = new HashSet<string>();
            Visit(scene, visited, result);
            if(rootAtTop)
                result.Reverse();
            return result;
        }

        public static List<string> GetSceneDependencies(string scene, ICollection<string> existingCollection, bool rootAtTop)
        {
            var result = GetSceneDependencies(scene, rootAtTop);

            foreach (var s in existingCollection)
            {
                if (!result.Contains(s))
                    result.Add(s);
            }

            return result;
        }

        private static void Visit(string scene, HashSet<string> visited, List<string> result)
        {
            if (!visited.Add(scene)) return;
            if (!TryGetSceneParent(scene, out var dependency)) return;
            
            Visit(dependency, visited, result);
            result.Add(scene);
        }

        public static bool TryGetSceneParent(string scene, out string parent)
        {
            return PersistentSceneDependency.TryGetValue(scene, out parent);
        }
#if UNITY_EDITOR
        public static void ValidateData()
        {
            var newDictionary =  NOSceneUtility.GetScenesInBuildSettings().ToDictionary(
                scene => scene,
                scene => PersistentSceneDependency.TryGetValue(scene, value: out var value)
                    ? value : String.Empty);
            PersistentSceneDependency = newDictionary;
        }
#endif
        void ValidateChange()
        {
            PersistentSceneDependency.Clear();
            foreach (var pair in DependencyTree)
            {
                PersistentSceneDependency[pair.SceneName] = pair.SceneParent;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            PersistentSceneDependency.Clear();
            foreach (var pair in DependencyTree)
            {
                PersistentSceneDependency[pair.SceneName] = pair.SceneParent;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            DependencyTree.Clear();
            foreach (var item in PersistentSceneDependency)
            {
                DependencyTree.Add(new SceneDependencyPair(item.Key, item.Value));
            }
        }

        [Serializable]
        private struct SceneDependencyPair
        {
            [SerializeField, HideLabel, ReadOnly] public string SceneName;

            [SerializeField, ValueDropdown(nameof(GetScenes), IsUniqueList = true)]
            public string SceneParent;

            public SceneDependencyPair(string key, string value)
            {
                SceneName = key;
                SceneParent = value;
            }
            private IEnumerable<string> GetScenes => NOSceneUtility.GetScenesInBuildSettings();
        }

        public static bool SceneDependsOn(string item, string dependency)
        {
            if (!PersistentSceneDependency.TryGetValue(item, out var dep)) return false;
            return dep == dependency || SceneDependsOn(dep, dependency);
        }
    }
}