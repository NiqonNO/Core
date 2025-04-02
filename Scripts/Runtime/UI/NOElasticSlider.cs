using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NiqonNO.Core.UI
{
    [AddComponentMenu("NiqonNO/UI/NOElasticSlider")]
    public class NOElasticSlider : NOSlider, IEndDragHandler
    {
        [SerializeField] private RectTransform HandleTip;
        
        [SerializeField] private SpringSetting HandleTipSpring;
        [Space]
        [SerializeField] private SpringSetting HandleBaseSpring;

        private bool IsDragging = false;
        private Vector2 TargetPosition = Vector2.zero;

        private bool IsIdle
        {
            get => HandleTipSpring.Idle && HandleBaseSpring.Idle;
            set => HandleTipSpring.Idle = HandleBaseSpring.Idle = value;
        }

        protected override void UpdateCachedReferences()
        {
            base.UpdateCachedReferences();
            TargetPosition = Vector2.zero;
            if (HandleTip)
            {
                SetHandleAnchorAndPosition(HandleTip, NormalizedValue);
            }
            if (HandleRect)
            {
                SetHandleAnchorAndPosition(HandleRect, NormalizedValue);
            }
            HandleTipSpring.Reset();
            HandleBaseSpring.Reset();
            IsDragging = false;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            IsDragging = true;
            base.OnDrag(eventData);
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            IsDragging = false;
            IsIdle = false;
        }
        protected override void Update()
        {
            if (DelayedUpdateVisuals)
            {
                base.Update();
                return;
            }

            UpdateVisuals();
        }
        
        protected override void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = HandleContainerRect ? HandleContainerRect : FillContainerRect;
            if (clickRect == null) return;
            
            Rect rect = clickRect.rect;
            if (rect.size[(int)SlideAxis] <= 0) return;
            
            Vector2 position = Vector2.zero;
            if (!NOMultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position)) return;

            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, position, cam, out localCursor)) return;
            
            localCursor -= rect.center;
            TargetPosition = localCursor - Offset;
        }
        protected override void UpdateHandle()
        {
            if (!IsDragging && IsIdle)
            {
                base.UpdateHandle();
                SetHandleAnchorAndPosition(HandleTip, NormalizedValue);
                return;
            }
            if (HandleContainerRect == null) return;

            Rect handleAreaRect = HandleContainerRect.rect;
            
            Vector2 handleTipLocalPosition = HandleTip.localPosition;
            Vector2 handleBaseLocalPosition = HandleRect.localPosition;

            Vector2 sliderHandleAnchorPosition = Vector2.zero;
            sliderHandleAnchorPosition[(int)SlideAxis] = SlideAxis == Axis.Horizontal
                ? Mathf.Lerp(handleAreaRect.xMin, handleAreaRect.xMax, ReverseValue ? 1 - NormalizedValue : NormalizedValue)
                : Mathf.Lerp(handleAreaRect.yMin, handleAreaRect.yMax, ReverseValue ? 1 - NormalizedValue : NormalizedValue);

            handleTipLocalPosition = HandleSpring(HandleTipSpring, handleTipLocalPosition, handleBaseLocalPosition, TargetPosition);
            handleBaseLocalPosition = HandleSpring(HandleBaseSpring, handleBaseLocalPosition, sliderHandleAnchorPosition, handleTipLocalPosition);
            
            SetHandleAnchorAndPosition(HandleTip, handleTipLocalPosition);
            SetHandleAnchorAndPosition(HandleRect, handleBaseLocalPosition);
            
            if (IsDragging)
            {
                float newValue = ReverseValue 
                    ? 1 - HandleRect.anchorMax[(int)SlideAxis] 
                    : HandleRect.anchorMax[(int)SlideAxis];
                Set(Mathf.Lerp(MinValue, MaxValue, newValue), true, false);
            }
        }
        
        protected override void SetHandleAnchorAndPosition(RectTransform rectTransform, float normalizedValue)
        {
            base.SetHandleAnchorAndPosition(rectTransform, normalizedValue);
            Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPosition);   
            rectTransform.anchoredPosition = Vector2.zero;
        }
        protected virtual void SetHandleAnchorAndPosition(RectTransform rectTransform, Vector2 position)
        {
            Rect rect = HandleContainerRect.rect;
            
            Vector2 clamped = SlideAxis == Axis.Horizontal 
                ? new Vector2(Mathf.Clamp(position.x, rect.xMin, rect.xMax), 0)
                : new Vector2(0, Mathf.Clamp(position.y, rect.yMin, rect.yMax));

            float normalizedValue = SlideAxis == Axis.Horizontal 
                ? Mathf.InverseLerp(rect.xMin, rect.xMax, clamped.x)
                : Mathf.InverseLerp(rect.yMin, rect.yMax, clamped.y);

            base.SetHandleAnchorAndPosition(rectTransform, ReverseValue ? 1 - normalizedValue : normalizedValue);
            Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPosition);
            rectTransform.anchoredPosition = new Vector2(position.x - clamped.x, position.y - clamped.y);
        }
        Vector2 HandleSpring(SpringSetting spring, Vector2 currentPosition, Vector2 originPosition, Vector2 targetPosition)
        {
            if (IsDragging && IsInteractable())
            {
                targetPosition = NormalizePosition(targetPosition);
                
                Vector2 fullDisplacement = TargetPosition - currentPosition;
                float displacementDirection = Mathf.Abs(Vector2.Dot(fullDisplacement.normalized, Vector2.right));
                
                fullDisplacement.x *= displacementDirection;
                fullDisplacement.y *= 1 - displacementDirection;
                
                targetPosition.x = Mathf.Abs(fullDisplacement.x) < spring.MoveThreshold ? originPosition.x : targetPosition.x;
                targetPosition.y = Mathf.Abs(fullDisplacement.y) < spring.MoveThreshold ? originPosition.y : targetPosition.y;
            }
            else
            {
                targetPosition = originPosition;
            }
            
            Vector2 dragDisplacement = targetPosition - currentPosition;
            Vector2 stretchDisplacement = currentPosition - originPosition;
            float dragDirection = Mathf.Clamp01(Vector2.Dot(dragDisplacement.normalized, stretchDisplacement.normalized));

            float damping = Mathf.Lerp(spring.RestDamping, spring.StretchDamping,
                stretchDisplacement.magnitude / spring.StretchRange * dragDirection);
            Vector2 dampingForce = -damping * spring.Velocity;

            Vector2 springForce = spring.SpringStrength * dragDisplacement;
            Vector2 acceleration = springForce + dampingForce;

            spring.Velocity += acceleration * Time.deltaTime;
            currentPosition += spring.Velocity * Time.deltaTime;

            if (spring.Velocity.magnitude < spring.RestThreshold)
            {
                spring.Reset();
            }
            else
            {
                spring.Idle = false;
            }

            return NormalizePosition(currentPosition);

            Vector2 NormalizePosition(Vector2 toNormalize)
            {
                Vector2 localPoint = toNormalize - originPosition;
                float range = spring.StretchRange;
                if (spring.LimitRangeOnEdges)
                {
                    range = Mathf.Lerp(spring.StretchRange, spring.LimitedStretchRange, Mathf.Abs(NormalizedValue * 2 - 1));
                }
                return originPosition + localPoint.normalized * Mathf.Min(localPoint.magnitude, range);
            }
        }

        [Serializable]
        private class SpringSetting
        {
            [field: SerializeField, 
                    MinValue(0)] 
            public float SpringStrength { get; private set; } = 500f;
            
            [field: SerializeField, 
                    BoxGroup("Damping"), 
                    LabelText("Resting"), 
                    MinValue(0)] 
            public float RestDamping { get; private set; } = 15f;
            
            [field: SerializeField, 
                    BoxGroup("Damping"), 
                    LabelText("Stretched"), 
                    MinValue(0)] 
            public float StretchDamping { get; private set; } = 50f;
            
            [field: SerializeField, 
                    BoxGroup("Stretch")] 
            public bool LimitRangeOnEdges { get; private set; }
            
            [field: SerializeField, 
                    BoxGroup("Stretch"), 
                    LabelText("Range"), 
                    MinValue(0)] 
            public float StretchRange { get; private set; } = 10f;
            
            [field: SerializeField, 
                    BoxGroup("Stretch"), 
                    LabelText("Limited Range"), 
                    ShowIf(nameof(LimitRangeOnEdges)), 
                    MinValue(0)] 
            public float LimitedStretchRange { get; private set; } = 2f;
            
            [field: SerializeField,  
                    BoxGroup("Threshold"), 
                    LabelText("To Move"), MinValue(0)] 
            public float MoveThreshold { get; private set; }
            
            [field: SerializeField,  
                    BoxGroup("Threshold"), 
                    LabelText("To Rest"), MinValue(0)] 
            public float RestThreshold { get; private set; } = 1f;

            public Vector2 Velocity { get; set; }
            public bool Idle { get; set; } = true;

            public void Reset()
            {
                Velocity = Vector2.zero;
                Idle = true;
            }
        }
    }
}