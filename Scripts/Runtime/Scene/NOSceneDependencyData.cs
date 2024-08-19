using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core.Scene
{
    [Serializable]
    public class NOSceneDependencyData : ISerializationCallbackReceiver
    {
        private static Dictionary<string, string[]> PersistentSceneDependency { get; set; } = new();
        
        [SerializeField, ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false), OnValueChanged(nameof(ValidateChange), true)]
        private List<SceneDependencyPair> DependencyTree = new();

        public static SortedSet<string> GetSceneDependencies(string scene)
        {
            return GetSceneDependencies(scene, new(new SceneDepthComparer(true)));
        }
        public static SortedSet<string> GetSceneDependencies(string scene,  SortedSet<string> sceneCollection)
        {
            if (sceneCollection.Contains(scene) || !PersistentSceneDependency.ContainsKey(scene)) return sceneCollection;
            sceneCollection.Add(scene);
            return PersistentSceneDependency[scene].Aggregate(sceneCollection, (current, sceneDependency) => GetSceneDependencies(sceneDependency, current));
        }
        
        public static void ValidateData()
        {
            var newDictionary =  Utility.NOUtility.GetScenesInBuildSettings().ToDictionary(
                scene => scene, 
                scene => PersistentSceneDependency.TryGetValue(scene, value: out var value) 
                    ? value : new string[0]);
            PersistentSceneDependency = newDictionary;
        }
        void ValidateChange()
        {
            PersistentSceneDependency.Clear();
            foreach (var pair in DependencyTree)
            {
                PersistentSceneDependency[pair.SceneName] = pair.SceneDependencies.Clone() as string[];
            }
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            PersistentSceneDependency.Clear();
            foreach (var pair in DependencyTree)
            {
                PersistentSceneDependency[pair.SceneName] = pair.SceneDependencies.Clone() as string[];
            }
        }
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            DependencyTree.Clear();
            foreach (var item in PersistentSceneDependency)
            {
                DependencyTree.Add(new SceneDependencyPair(item.Key, item.Value.Clone() as string[]));
            }
        }

        [Serializable]
        private struct SceneDependencyPair
        {
            [SerializeField, HideLabel, ReadOnly]
            public string SceneName;
            [SerializeField, ValueDropdown(nameof(GetScenes), IsUniqueList = true)]
            public string[] SceneDependencies;

            public SceneDependencyPair(string key, string[] values)
            {
                SceneName = key;
                SceneDependencies = values;
            }
            private IEnumerable<string> GetScenes => Utility.NOUtility.GetScenesInBuildSettings();
        }
        
        internal class SceneDepthComparer : IComparer<string>
        {
            private int Direction;
            
            public SceneDepthComparer(bool topFirst)
            {
                Direction = topFirst ? 1 : -1;
            }
            
            public int Compare(string x, string y)
            {
                if (x == y)
                    return 0;
                if (SceneDependsOn(x, y))
                    return 1 * Direction;
                if (SceneDependsOn(y, x))
                    return -1 * Direction;
                return string.Compare(x, y, StringComparison.Ordinal) * Direction;
            }
        }

        public static bool SceneDependsOn(string item, string dependency)
        {
            if (PersistentSceneDependency.TryGetValue(item, out var deps))
            {
                foreach (var dep in deps)
                {
                    if (dep == dependency || SceneDependsOn(dep, dependency))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}