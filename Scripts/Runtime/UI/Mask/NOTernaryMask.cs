using System;
using NiqonNO.Core.Utility;
using UnityEngine;

namespace NiqonNO.Core.UI
{
    [AddComponentMenu("NiqonNO/UI/NOTernaryMask")]
    public class NOTernaryMask : NOMask
    {
        private static readonly int SliderTopCorner = Shader.PropertyToID("_SliderTopCorner");
        private static readonly int SliderBottomCorners = Shader.PropertyToID("_SliderBottomCorners");
        private static readonly int HandlePositions = Shader.PropertyToID("_HandlePositions");

        private RectTransform ForegroundContainerRect;
        private RectTransform HandleRect;

        [NonSerialized] private Material BaseMaterial;
        [NonSerialized] private Material InstancedMaterial;

        public void SetData(RectTransform foregroundContainerRect, RectTransform handleRect)
        {
            ForegroundContainerRect = foregroundContainerRect;
            HandleRect = handleRect;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateMaterialProperties();
        }


        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            if (BaseMaterial != baseMaterial)
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

        public void UpdateMaterialProperties()
        {
            if (MaskMaterial == null)
                return;

            Vector2 localSize = RectTransform.rect.size;
            Vector2 topVert = Vector2.zero;
            Vector2 leftVert = Vector2.zero;
            Vector2 rightVert = Vector2.zero;
            Vector2 handlePos = Vector2.zero;

            if (ForegroundContainerRect != null)
            {
                Vector2 rectSize = ForegroundContainerRect.rect.size;
                Vector2 rectCenter = ForegroundContainerRect.rect.center;

                float width = rectSize.x;
                float height = width * Mathf.Sqrt(3) / 2;
                if (height > rectSize.y)
                {
                    height = rectSize.y;
                    width = 2 * height / Mathf.Sqrt(3);
                }

                float halfHeight = (height / 2);
                float halfWidth = (width / 2);

                leftVert = transform.InverseTransformPoint(
                    ForegroundContainerRect.TransformPoint((rectCenter + new Vector2(-halfWidth, -halfHeight))));
                topVert = transform.InverseTransformPoint(
                    ForegroundContainerRect.TransformPoint((rectCenter + new Vector2(0, halfHeight))));
                rightVert = transform.InverseTransformPoint(
                    ForegroundContainerRect.TransformPoint((rectCenter + new Vector2(halfWidth, -halfHeight))));
            }

            if (HandleRect != null)
            {
                handlePos = transform.InverseTransformPoint(HandleRect.position);
            }

            MaskMaterial.SetVector(SliderTopCorner,
                new Vector4(topVert.x, topVert.y, 0, 0));
            MaskMaterial.SetVector(SliderBottomCorners,
                new Vector4(leftVert.x, leftVert.y, rightVert.x, rightVert.y));
            MaskMaterial.SetVector(HandlePositions,
                new Vector4(handlePos.x, handlePos.y, localSize.x, localSize.y));
        }
    }
}
