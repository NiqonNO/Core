using System;
using UnityEngine;

namespace NiqonNO.Core.UI
{
    [AddComponentMenu("NiqonNO/UI/NOSliderMask")]
    public class NOSliderMask : NOMask
    {
        private static readonly int SliderCorners = Shader.PropertyToID("_SliderCorners");
        private static readonly int HandlePositions = Shader.PropertyToID("_HandlePositions");

        private RectTransform ForegroundContainerRect;
        private RectTransform HandleRect;

        [NonSerialized]
        private Material BaseMaterial;
        [NonSerialized]
        private Material InstancedMaterial;

        private int SliderAxis;

        public void SetData(RectTransform foregroundContainerRect, RectTransform handleRect, int sliderAxis)
        {
            ForegroundContainerRect = foregroundContainerRect;
            HandleRect = handleRect;
            SliderAxis = sliderAxis;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateMaterialProperties();
        }
        

        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            if(BaseMaterial != baseMaterial)
            {
                BaseMaterial = baseMaterial;
#if UNITY_EDITOR
                DestroyImmediate(InstancedMaterial);
#else
                Destroy(InstancedMaterial);
#endif
                InstancedMaterial = Instantiate(BaseMaterial);
            }
            var material = base.GetModifiedMaterial(InstancedMaterial);
            UpdateMaterialProperties();
            return material;
        }

        private void UpdateMaterialProperties()
        {
            UpdateMaterialProperties(SliderAxis);
        }
        public void UpdateMaterialProperties(int sliderAxis)
        {
            if (MaskMaterial == null)
                return;

            Vector2 localSize = RectTransform.rect.size;
            Vector2 minEdge = Vector2.zero;
            Vector2 maxEdge = Vector2.zero;
            Vector2 handlePos = Vector2.zero;

            minEdge[sliderAxis] = 1;
            maxEdge[sliderAxis] = 1;
            
            if (ForegroundContainerRect != null)
            {
                minEdge *= transform.InverseTransformPoint(
                    ForegroundContainerRect.TransformPoint(ForegroundContainerRect.rect.min));
                maxEdge *=transform.InverseTransformPoint(
                    ForegroundContainerRect.TransformPoint(ForegroundContainerRect.rect.max));
            }

            if (HandleRect != null)
            {
                handlePos = transform.InverseTransformPoint(HandleRect.position);
            }

            MaskMaterial.SetVector(SliderCorners,
                new Vector4(minEdge.x, minEdge.y, maxEdge.x, maxEdge.y));
            MaskMaterial.SetVector(HandlePositions,
                new Vector4(handlePos.x, handlePos.y, localSize.x, localSize.y));
        }
    }
}