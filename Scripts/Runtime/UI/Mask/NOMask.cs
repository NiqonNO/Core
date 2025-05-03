using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace NiqonNO.Core.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public abstract class NOMask : Mask
    {
        public RectTransform RectTransform => rectTransform;
        public Graphic Graphic => graphic;

        public bool ShowMaskGraphic => showMaskGraphic;

        [NonSerialized,ShowInInspector] protected Material MaskMaterial;

        [NonSerialized] private Material UnmaskMaterial;

        protected NOMask()
        {
        }

        public override bool MaskEnabled()
        {
            return IsActive() && Graphic != null;
        }

        protected override void OnEnable()
        {
            //base.OnEnable();
            if (Graphic != null)
            {
                Graphic.canvasRenderer.hasPopInstruction = true;
                Graphic.SetMaterialDirty();

                if (Graphic is MaskableGraphic maskableGraphic)
                    maskableGraphic.isMaskingGraphic = true;
            }

            MaskUtilities.NotifyStencilStateChanged(this);
        }

        protected override void OnDisable()
        {
            //base.OnDisable();
            if (Graphic != null)
            {
                Graphic.SetMaterialDirty();
                Graphic.canvasRenderer.hasPopInstruction = false;
                Graphic.canvasRenderer.popMaterialCount = 0;

                if (Graphic is MaskableGraphic maskableGraphic)
                    maskableGraphic.isMaskingGraphic = false;
            }

            StencilMaterial.Remove(MaskMaterial);
            MaskMaterial = null;
            StencilMaterial.Remove(UnmaskMaterial);
            UnmaskMaterial = null;

            MaskUtilities.NotifyStencilStateChanged(this);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            //base.OnValidate();
            if (!IsActive())
                return;

            if (Graphic != null)
            {
                if (Graphic is MaskableGraphic maskableGraphic)
                    maskableGraphic.isMaskingGraphic = true;

                Graphic.SetMaterialDirty();
            }

            MaskUtilities.NotifyStencilStateChanged(this);
        }

#endif

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return !isActiveAndEnabled ||
                   RectTransformUtility.RectangleContainsScreenPoint(RectTransform, sp, eventCamera);
        }

        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!MaskEnabled())
                return baseMaterial;

            var rootSortCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
            var stencilDepth = MaskUtilities.GetStencilDepth(transform, rootSortCanvas);
            if (stencilDepth >= 8)
            {
                Debug.LogWarning("Attempting to use a stencil mask with depth > 8", gameObject);
                return baseMaterial;
            }

            int desiredStencilBit = 1 << stencilDepth;

            if (desiredStencilBit == 1)
            {
                var maskMaterial = StencilMaterial.Add(baseMaterial, 1, StencilOp.Replace, CompareFunction.Always,
                    ShowMaskGraphic ? ColorWriteMask.All : 0);
                StencilMaterial.Remove(MaskMaterial);
                MaskMaterial = maskMaterial;

                var unmaskMaterial = StencilMaterial.Add(baseMaterial, 1, StencilOp.Zero, CompareFunction.Always, 0);
                StencilMaterial.Remove(UnmaskMaterial);
                UnmaskMaterial = unmaskMaterial;
                Graphic.canvasRenderer.popMaterialCount = 1;
                Graphic.canvasRenderer.SetPopMaterial(UnmaskMaterial, 0);

                return MaskMaterial;
            }

            var maskMaterial2 = StencilMaterial.Add(baseMaterial, desiredStencilBit | (desiredStencilBit - 1),
                StencilOp.Replace, CompareFunction.Equal, ShowMaskGraphic ? ColorWriteMask.All : 0,
                desiredStencilBit - 1,
                desiredStencilBit | (desiredStencilBit - 1));
            StencilMaterial.Remove(MaskMaterial);
            MaskMaterial = maskMaterial2;

            Graphic.canvasRenderer.hasPopInstruction = true;
            var unmaskMaterial2 = StencilMaterial.Add(baseMaterial, desiredStencilBit - 1, StencilOp.Replace,
                CompareFunction.Equal, 0, desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
            StencilMaterial.Remove(UnmaskMaterial);
            UnmaskMaterial = unmaskMaterial2;
            Graphic.canvasRenderer.popMaterialCount = 1;
            Graphic.canvasRenderer.SetPopMaterial(UnmaskMaterial, 0);

            return MaskMaterial;
        }
    }
}