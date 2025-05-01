using NiqonNO.Core.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NiqonNO.Core.UI
{
    [AddComponentMenu("NiqonNO/UI/NOTernary")]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class NOTernarySlider : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        [SerializeField] 
        private RectTransform _HandleRect;
        public RectTransform HandleRect
        {
            get => _HandleRect;
            set => NOSetPropertyUtility.SetClass(ref _HandleRect, value, () =>
            {
                UpdateCachedReferences();
                UpdateVisuals();
            });
        }

        [SerializeField] 
        private float _MinValue = 0;
        public float MinValue
        {
            get => _MinValue;
            set => NOSetPropertyUtility.SetStruct(ref _MinValue, value, () =>
            {
                Set(_Value.Value);
                UpdateVisuals();
            });
        }

        [SerializeField] 
        private float _MaxValue = 1;
        public float MaxValue
        {
            get => _MaxValue;
            set => NOSetPropertyUtility.SetStruct(ref _MaxValue, value, () =>
            {
                Set(_Value.Value);
                UpdateVisuals();
            });
        }

        [SerializeField] 
        private bool _WholeNumbers = false;
        public bool WholeNumbers
        {
            get => _WholeNumbers;
            set => NOSetPropertyUtility.SetStruct(ref _WholeNumbers, value, () =>
            {
                Set(_Value.Value);
                UpdateVisuals();
            });
        }

        [SerializeField] 
        private NOVector3Variable _Value;
        public virtual Vector3 Value
        {
            get => WholeNumbers ? 
                NOMath2D.RoundBarycentric(_Value.Value) : _Value.Value;
            set => Set(value);
        }
        public virtual Vector3 NormalizedValue
        {
            get => Mathf.Approximately(MinValue, MaxValue)
                ? Vector3.one / 3f
                : NOMath2D.InverseLerpBarycentric(MinValue, MaxValue, Value);
            set => Value = NOMath2D.LerpBarycentric(MinValue, MaxValue, value);
        }

        [Space] 
        [SerializeField] 
        private UnityEvent<Vector3> _OnValueChanged = new();
        public UnityEvent<Vector3> OnValueChanged
        {
            get => _OnValueChanged;
            set => _OnValueChanged = value;
        }

        private Transform HandleTransform;
        protected RectTransform HandleContainerRect;

        protected Vector2 Offset = Vector2.zero;

        private Vector2[] SlideAreaCorners = new Vector2[3];

#pragma warning disable 649
        protected DrivenRectTransformTracker Tracker;
#pragma warning restore 649

        protected bool DelayedUpdateVisuals = false;

        private float StepSize => WholeNumbers ? 1 : (MaxValue - MinValue) * 0.1f;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (WholeNumbers)
            {
                _MinValue = Mathf.Round(_MinValue);
                _MaxValue = Mathf.Round(_MaxValue);
            }

            if (IsActive())
            {
                UpdateCachedReferences();
                DelayedUpdateVisuals = true;
            }

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                OnValueChanged.Invoke(Value);
#endif
        }

        public virtual void LayoutComplete()
        {
        }

        public virtual void GraphicUpdateComplete()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCachedReferences();
            Set(_Value.Value, false);
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            Tracker.Clear();
            base.OnDisable();
        }

        protected virtual void Update()
        {
            if (DelayedUpdateVisuals)
            {
                DelayedUpdateVisuals = false;
                Set(_Value.Value, false);
                UpdateVisuals();
            }
        }

        protected override void OnDidApplyAnimationProperties()
        {
            _Value.Value = ClampValue(_Value.Value);
            Vector3 oldNormalizedValue = NormalizedValue;
            GetSlideAreaCorners();
            if (HandleContainerRect != null)
                oldNormalizedValue = NOMath2D.GetBarycentricCoordinates(HandleRect.anchoredPosition, SlideAreaCorners[0],
                    SlideAreaCorners[1], SlideAreaCorners[2]);

            UpdateVisuals();

            if (oldNormalizedValue != NormalizedValue)
            {
                UISystemProfilerApi.AddMarker("Ternary.value", this);
                OnValueChanged.Invoke(_Value.Value);
            }

            base.OnDidApplyAnimationProperties();
        }

        protected virtual void UpdateCachedReferences()
        {
            if (_HandleRect && _HandleRect != (RectTransform)transform)
            {
                HandleTransform = _HandleRect.transform;
                if (HandleTransform.parent != null)
                    HandleContainerRect = HandleTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                _HandleRect = null;
                HandleContainerRect = null;
            }

            GetSlideAreaCorners();
        }

        Vector3 ClampValue(Vector3 input)
        {
            Vector3 newValue = NOMath2D.ClampBarycentric(MinValue, MaxValue, input);
            if (WholeNumbers)
                newValue =  NOMath2D.RoundBarycentric(newValue);
            return newValue;
        }

        public virtual void SetValueWithoutNotify(Vector3 input)
        {
            Set(input, false);
        }

        protected virtual void Set(Vector3 input, bool sendCallback = true, bool updateVisuals = true)
        {
            Vector3 newValue = ClampValue(input);

            if (_Value.Value == newValue)
                return;

            _Value.Value = newValue;
            if (updateVisuals)
            {
                UpdateVisuals();
            }

            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Ternary.value", this);
                _OnValueChanged.Invoke(newValue);
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            if (!IsActive())
                return;

            UpdateVisuals();
        }

        protected void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            Tracker.Clear();

            UpdateHandle();
            GetSlideAreaCorners();
        }

        private void GetSlideAreaCorners()
        {
            if (HandleContainerRect == null)
            {
                SlideAreaCorners[0] = SlideAreaCorners[1] = SlideAreaCorners[2] = Vector2.zero;
                return;
            }

            Vector2 rectSize = HandleContainerRect.rect.size;
            Vector2 rectCenter = HandleContainerRect.rect.center;

            float width = rectSize.x;
            float height = width * Mathf.Sqrt(3) / 2;
            if (height > rectSize.y)
            {
                height = rectSize.y;
                width = 2 * height / Mathf.Sqrt(3);
            }

            float halfHeight = (height / 2);
            float halfWidth = (width / 2);

            SlideAreaCorners[0] = (rectCenter + new Vector2(-halfWidth, -halfHeight)) / rectSize;
            SlideAreaCorners[1] = (rectCenter + new Vector2(0, halfHeight)) / rectSize;
            SlideAreaCorners[2] = (rectCenter + new Vector2(halfWidth, -halfHeight)) / rectSize;
        }

        protected virtual void UpdateHandle()
        {
            if (HandleContainerRect == null) return;

            SetHandleAnchorAndPosition(_HandleRect, NormalizedValue);
        }

        protected virtual void SetHandleAnchorAndPosition(RectTransform rectTransform, Vector3 normalizedValue)
        {
            Tracker.Add(this, rectTransform, DrivenTransformProperties.Anchors);

            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.one;
            anchorMin =
                anchorMax = NOMath2D.GetPositionFromBarycentric(normalizedValue, SlideAreaCorners[0], SlideAreaCorners[1],
                    SlideAreaCorners[2]);
            rectTransform.anchorMax = anchorMax;
            rectTransform.anchorMin = anchorMin;
        }

        protected virtual void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            InternalUpdateDrag(eventData, cam);
        }

        private void InternalUpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = HandleContainerRect;
            if (clickRect != null && clickRect.rect.size.magnitude > 0)
            {
                Vector2 position = Vector2.zero;
                if (!NOMultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
                    return;

                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, position, cam, out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                Vector3 val = NOMath2D.GetBarycentricCoordinates((localCursor - Offset) / clickRect.rect.size,
                    SlideAreaCorners[0], SlideAreaCorners[1], SlideAreaCorners[2]);
                NormalizedValue = val;
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            Offset = Vector2.zero;
            if (HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(_HandleRect,
                    eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_HandleRect,
                        eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out localMousePos))
                    Offset = localMousePos;
            }
            else
            {
                InternalUpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;
            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    if (FindSelectableOnLeft() == null)
                        Set(Value + Vector3.left * StepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Right:
                    if (FindSelectableOnRight() == null)
                        Set(Value + Vector3.back * StepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Up:
                    if (FindSelectableOnUp() == null)
                        Set(Value + Vector3.up * StepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Down:
                    if (FindSelectableOnDown() == null)
                        Set(Value - Vector3.up * StepSize);
                    else
                        base.OnMove(eventData);
                    break;
            }
        }

        public override Selectable FindSelectableOnLeft()
        {
            return null;
            /*if (navigation.mode == Navigation.Mode.Automatic && SlideAxis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnLeft();*/
        }

        public override Selectable FindSelectableOnRight()
        {
            return null;
            /*if (navigation.mode == Navigation.Mode.Automatic && SlideAxis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnRight();*/
        }

        public override Selectable FindSelectableOnUp()
        {
            return null;
            /*if (navigation.mode == Navigation.Mode.Automatic && SlideAxis == Axis.Vertical)
                return null;
            return base.FindSelectableOnUp();*/
        }

        public override Selectable FindSelectableOnDown()
        {
            return null;
            /*if (navigation.mode == Navigation.Mode.Automatic && SlideAxis == Axis.Vertical)
                return null;
            return base.FindSelectableOnDown();*/
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 rectSize = HandleContainerRect.rect.size;

            Vector3 v0, v1, v2, v01, v12, v20;

            v0 = HandleContainerRect.TransformPoint(SlideAreaCorners[0] * rectSize);
            v1 = HandleContainerRect.TransformPoint(SlideAreaCorners[1] * rectSize);
            v2 = HandleContainerRect.TransformPoint(SlideAreaCorners[2] * rectSize);

            v01 = (v0 + v1) / 2;
            v12 = (v1 + v2) / 2;
            v20 = (v2 + v0) / 2;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v0, v12);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(v2, v0);
            Gizmos.DrawLine(v1, v20);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v2, v01);

        }
    }
}