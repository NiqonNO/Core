using System;
using System.Collections.Generic;
using System.Linq;
using NiqonNO.Core.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core.Scene
{
    public class NOSceneDependencyTree : NOScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField, ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false),
         OnValueChanged(nameof(SetToStaticContext), true)]
        private List<SceneDependencyPair> DependencyTree = new();
        
        public void SetToStaticContext() => NOSceneDependencyData.SetToStaticContext(DependencyTree);

        void ISerializationCallbackReceiver.OnBeforeSerialize() => NOSceneDependencyData.SetFromStaticContext(ref DependencyTree);
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
        
        [Serializable]
        public struct SceneDependencyPair
        {
            [SerializeField, HideLabel, ReadOnly] public string SceneName;

            [SerializeField, ValueDropdown(nameof(GetScenes), IsUniqueList = true)]
            public string SceneParent;

            public SceneDependencyPair(string key, string value)
            {
                SceneName = key;
                SceneParent = value;
            }

            private IEnumerable<string> GetScenes => NOSceneUtility.GetScenesInBuildSettings().Prepend("");
        }

    }
}