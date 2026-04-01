using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace NiqonNO.Core.Editor
{
public class NOArmatureSetterToolTab : NOEditorUtilityToolTab
    {
        public override string TabName => "Armature Setter";
        public override int Order => 1;
        
        [ShowInInspector, TabGroup("Replace Armature"), PropertyOrder(0)] 
        private Transform ArmatureRoot;

        [ShowInInspector, TabGroup("Replace Armature"), TabGroup("Find Armature"), PropertyOrder(1)] 
        private string RootBoneName = "RL_BoneRoot";

        [ShowInInspector, TabGroup("Replace Armature"), PropertyOrder(2)] 
        private SkinnedMeshRenderer ProxyMesh;
        
        [ShowInInspector, TabGroup("Replace Armature"), TabGroup("Find Armature"), PropertyOrder(10)]  
        public List<SkinnedMeshRenderer> SourceRenderers = new();

        [Button, TabGroup("Replace Armature"), PropertyOrder(100)]
        void ReplaceArmature()
        {
            if (!ArmatureRoot)
            {
                Debug.LogError("[Armature Setter] ArmatureRoot is null.");
                return;
            }
            if (!ProxyMesh)
            {
                Debug.LogError("[Armature Setter] ProxyMesh is null.");
                return;
            }
            if (SourceRenderers.IsNullOrEmpty())            
            {
                Debug.LogError("[Armature Setter] no SourceRenderers to set.");
                return;
            }
            var rootBone = FindRecursive(ArmatureRoot, RootBoneName);
            if (!rootBone)
            {
                Debug.LogError($"[Armature Setter] Could not find a root bone by name {RootBoneName}.");
                return;
            }
            var newArmatureBones = ArmatureRoot.GetComponentsInChildren<Transform>();

            foreach (var meshRenderer in SourceRenderers)
            {
                if(!meshRenderer) continue;
                
                meshRenderer.rootBone = rootBone;
                SetArmatureToRenderer(newArmatureBones, meshRenderer, ProxyMesh);
            }
        }
        
        [ShowInInspector, TabGroup("Find Armature"), PropertyOrder(0)]
        private string ArmatureName = "Armature";
        
        [Button, TabGroup("Find Armature"), PropertyOrder(100)]
        void FindArmature()
        {
            if (SourceRenderers.IsNullOrEmpty())            
            {
                Debug.LogError("[Armature Setter] no SourceRenderers to set.");
                return;
            }
            
            foreach (var meshRenderer in SourceRenderers)
            {
                if(!meshRenderer) continue;

                var armatureRoot = FindRecursiveParent(meshRenderer.transform, ArmatureName);
                if (!armatureRoot)
                {
                    Debug.LogError($"[Armature Setter] Could not find a armature by name {ArmatureName}.");
                    return;
                }
                var rootBone = FindRecursive(armatureRoot, RootBoneName);
                if (!rootBone)
                {
                    Debug.LogError($"[Armature Setter] Could not find a root bone by name {RootBoneName}.");
                    return;
                }
                
                var newArmatureBones = armatureRoot.GetComponentsInChildren<Transform>();
                meshRenderer.rootBone = rootBone;
                SetArmatureToRenderer(newArmatureBones, meshRenderer);
            }
        }
        
        void SetArmatureToRenderer(Transform[] newArmatureBones, SkinnedMeshRenderer meshRenderer, SkinnedMeshRenderer proxyMesh)
        {
            if (proxyMesh == null)
            {
                SetArmatureToRenderer(newArmatureBones, meshRenderer);
                return;
            }

            var meshRendererBones = proxyMesh.bones;
            for (var i = 0; i < meshRendererBones.Length; i++)
            {
                foreach (var bone in newArmatureBones)
                {
                    if (meshRendererBones[i].name != bone.name) continue;
                    meshRendererBones[i] = bone;
                    break;
                }
            }
            meshRenderer.bones = meshRendererBones;
        }
        void SetArmatureToRenderer(Transform[] newArmatureBones, SkinnedMeshRenderer meshRenderer)
        {
            var meshRendererBones = new Transform[meshRenderer.sharedMesh.bindposes.Length];
            for (var i = 0; i < meshRendererBones.Length; i++)
            {
                var targetBindpose = meshRenderer.sharedMesh.bindposes[i];
                foreach (var bone in newArmatureBones)
                {
                    var computedBindpose = bone.worldToLocalMatrix;// * meshRenderer.rootBone.localToWorldMatrix;
                    if (!MatricesApproximatelyEqual(computedBindpose, targetBindpose)) continue;
                    meshRendererBones[i] = bone;
                    break;
                }
            }
            meshRenderer.bones = meshRendererBones;
        }

        bool MatricesApproximatelyEqual(Matrix4x4 a, Matrix4x4 b, float tolerance = 0.0001f)
        {
            for (int i = 0; i < 16; i++)
            {
                if (Mathf.Abs(a[i] - b[i]) > tolerance)
                    return false;
            }
            return true;
        }
        
        private Transform FindRecursive(Transform parent, string targetName)
        {
            if (parent.name == targetName)
                return parent;

            foreach (Transform child in parent)
            {
                var result = FindRecursive(child, targetName);
                if (result) return result;
            }

            return null;
        }
        
        private Transform FindRecursiveParent(Transform child, string targetName)
        {
            if (child.name == targetName)
                return child;
            
            return FindRecursive(child.parent, targetName);
        }
	}
}