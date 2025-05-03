using NiqonNO.Core.UI.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NiqonNO.Core.UI
{
    [AddComponentMenu("NiqonNO/UI/NOSlider")]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class  NOSlider : NOSelectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        [SerializeField]
        private RectTransform _FillRect;
        public RectTransform FillRect {
            get => _FillRect;
            set => NOSetPropertyUtility.SetClass(ref _FillRect, value, () => {
                    UpdateCachedReferences();
                    UpdateVisuals(); }); }

        [SerializeField]
        private RectTransform _HandleRect;
        public RectTransform HandleRect {
            get => _HandleRect;
            set => NOSetPropertyUtility.SetClass(ref _HandleRect, value, () => {
                UpdateCachedReferences();
                UpdateVisuals(); }); }
        
        [SerializeField]
        private NOSliderMask _ForegroundMask;
        public NOSliderMask ForegroundMask {
            get => _ForegroundMask;
            set => NOSetPropertyUtility.SetClass(ref _ForegroundMask, value, () => {
                UpdateCachedReferences();
                UpdateVisuals(); }); }

        [Space]
        [SerializeField]
        private Slider.Direction _Direction = Slider.Direction.LeftToRight;
        public Slider.Direction Direction {
            get => _Direction;
            set => NOSetPropertyUtility.SetStruct(ref _Direction, value, UpdateVisuals); }

        [SerializeField]
        private float _MinValue = 0;
        public float MinValue {
            get => _MinValue;
            set => NOSetPropertyUtility.SetStruct(ref _MinValue, value, () => {
                Set(_Value.Value);
                UpdateVisuals(); }); }

        [SerializeField]
        private float _MaxValue = 1;
        public float MaxValue {
            get => _MaxValue;
            set => NOSetPropertyUtility.SetStruct(ref _MaxValue, value, () => {
                Set(_Value.Value);
                UpdateVisuals(); }); } 

        [SerializeField]
        private bool _WholeNumbers = false;
        public bool WholeNumbers {
            get => _WholeNumbers;
            set => NOSetPropertyUtility.SetStruct(ref _WholeNumbers, value, () => {
                Set(_Value.Value);
                UpdateVisuals(); }); }

        [SerializeField]
        private NOFloatVariable _Value;
        public virtual float Value {
            get => WholeNumbers ? Mathf.Round(_Value.Value) : _Value.Value;
            set => Set(value); }
        public virtual float NormalizedValue { 
            get => Mathf.Approximately(MinValue, MaxValue) ? 0 : Mathf.InverseLerp(MinValue, MaxValue, Value);
            set => Value = Mathf.Lerp(MinValue, MaxValue, value); }
        
        [Space]
        [SerializeField]
        private UnityEvent<float> _OnValueChanged = new ();
        public UnityEvent<float> OnValueChanged {
            get => _OnValueChanged;
            set => _OnValueChanged = value;
        }
        
        private Image FillImage;
        private Transform FillTransform;
        protected RectTransform FillContainerRect;
        
        private Transform HandleTransform;
        protected RectTransform HandleContainerRect;
        
        protected Vector2 Offset = Vector2.zero;
        
        protected Axis SlideAxis => (_Direction == Slider.Direction.LeftToRight || _Direction == Slider.Direction.RightToLeft) ? Axis.Horizontal : Axis.Vertical;
        protected bool ReverseValue => _Direction == Slider.Direction.RightToLeft || _Direction == Slider.Direction.TopToBottom;
        
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
        {}
        
        public virtual void GraphicUpdateComplete()
        {}

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
            float oldNormalizedValue = NormalizedValue;
            if (FillContainerRect != null)
            {
                if (FillImage != null && FillImage.type == Image.Type.Filled)
                    oldNormalizedValue = FillImage.fillAmount;
                else
                    oldNormalizedValue = (ReverseValue ? 1 - _FillRect.anchorMin[(int)SlideAxis] : _FillRect.anchorMax[(int)SlideAxis]);
            }
            else if (HandleContainerRect != null)
                oldNormalizedValue = (ReverseValue ? 1 - _HandleRect.anchorMin[(int)SlideAxis] : _HandleRect.anchorMin[(int)SlideAxis]);

            UpdateVisuals();

            if (oldNormalizedValue != NormalizedValue)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
                OnValueChanged.Invoke(_Value.Value);
            }
            base.OnDidApplyAnimationProperties();
        }

        protected virtual void UpdateCachedReferences()
        {
            if (_FillRect && _FillRect != (RectTransform)transform)
            {
                FillTransform = _FillRect.transform;
                FillImage = _FillRect.GetComponent<Image>();
                if (FillTransform.parent != null)
                    FillContainerRect = FillTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                _FillRect = null;
                FillContainerRect = null;
                FillImage = null;
            }
            
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

            if (_ForegroundMask)
            {
                _ForegroundMask.SetData(HandleContainerRect, _HandleRect, (int)SlideAxis);
            }
        }

        float ClampValue(float input)
        {
            float newValue = Mathf.Clamp(input, MinValue, MaxValue);
            if (WholeNumbers)
                newValue = Mathf.Round(newValue);
            return newValue;
        }

        public virtual void SetValueWithoutNotify(float input)
        {
            Set(input, false);
        }
        protected virtual void Set(float input, bool sendCallback = true, bool updateVisuals = true)
        {
            float newValue = ClampValue(input);

            if (_Value.Value == newValue)
                return;

            _Value.Value = newValue;
            if(updateVisuals)
            {
                UpdateVisuals();
            }
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
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

        protected enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }

        protected void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            Tracker.Clear();
            
            UpdateHandle();
            UpdateFill();
            UpdateMask();
        }

        protected virtual void UpdateFill()
        {
            if (FillContainerRect == null) return;
            Tracker.Add(this, _FillRect, DrivenTransformProperties.Anchors);
            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.one;

            if (FillImage != null && FillImage.type == Image.Type.Filled)
            {
                FillImage.fillAmount = NormalizedValue;
            }
            else
            {
                if (ReverseValue)
                    anchorMin[(int)SlideAxis] = 1 - NormalizedValue;
                else
                    anchorMax[(int)SlideAxis] = NormalizedValue;
            }

            _FillRect.anchorMin = anchorMin;
            _FillRect.anchorMax = anchorMax;
        }
        protected virtual void UpdateHandle()
        {
            if (HandleContainerRect == null) return;

            SetHandleAnchorAndPosition(_HandleRect, NormalizedValue);
        }

        protected virtual void UpdateMask()
        {
            if (_ForegroundMask == null) return;
            _ForegroundMask.UpdateMaterialProperties((int)SlideAxis);
        }

        protected virtual void SetHandleAnchorAndPosition(RectTransform rectTransform, float normalizedValue)
        {
            Tracker.Add(this, rectTransform, DrivenTransformProperties.Anchors);

            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.one;
            anchorMin[(int)SlideAxis] =
                anchorMax[(int)SlideAxis] = (ReverseValue ? (1 - normalizedValue) : normalizedValue);
            rectTransform.anchorMax = anchorMax;
            rectTransform.anchorMin = anchorMin;
        }
        
        protected virtual void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            InternalUpdateDrag(eventData, cam);
        }

        private void InternalUpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = HandleContainerRect ?? FillContainerRect;
            if (clickRect != null && clickRect.rect.size[(int)SlideAxis] > 0)
            {
                Vector2 position = Vector2.zero;
                if (!NOMultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
                    return;

                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, position, cam, out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                float val = Mathf.Clamp01((localCursor - Offset)[(int)SlideAxis] / clickRect.rect.size[(int)SlideAxis]);
                NormalizedValue = (ReverseValue ? 1f - val : val);
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
            if (HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(_HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out localMousePos))
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
                case MoveDirection.Left when SlideAxis == Axis.Horizontal && FindSelectableOnLeft() == null:
                    Set(ReverseValue ? Value + StepSize : Value - StepSize);
                    break;
                case MoveDirection.Right when SlideAxis == Axis.Horizontal && FindSelectableOnRight() == null:
                    Set(ReverseValue ? Value - StepSize : Value + StepSize);
                    break;
                case MoveDirection.Up when SlideAxis == Axis.Vertical && FindSelectableOnUp() == null:
                    Set(ReverseValue ? Value - StepSize : Value + StepSize);
                    break;
                case MoveDirection.Down when SlideAxis == Axis.Vertical && FindSelectableOnDown() == null:
                    Set(ReverseValue ? Value + StepSize : Value - StepSize);
                    break;
                default:
                    base.OnMove(eventData);
                    break;
            }
        }

        public override Selectable FindSelectableOnLeft()
        {
            if (navigation.mode == Navigation.Mode.Automatic && SlideAxis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnLeft();
        }
        
        public override Selectable FindSelectableOnRight()
        {
            if (navigation.mode == Navigation.Mode.Automatic && SlideAxis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnRight();
        }

        public override Selectable FindSelectableOnUp()
        {
            if (navigation.mode == Navigation.Mode.Automatic && SlideAxis == Axis.Vertical)
                return null;
            return base.FindSelectableOnUp();
        }

        public override Selectable FindSelectableOnDown()
        {
            if (navigation.mode == Navigation.Mode.Automatic && SlideAxis == Axis.Vertical)
                return null;
            return base.FindSelectableOnDown();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void SetDirection(Slider.Direction direction, bool includeRectLayouts)
        {
            Axis oldAxis = SlideAxis;
            bool oldReverse = ReverseValue;
            this.Direction = direction;

            if (!includeRectLayouts)
                return;

            if (SlideAxis != oldAxis)
                RectTransformUtility.FlipLayoutAxes(transform as RectTransform, true, true);

            if (ReverseValue != oldReverse)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, (int)SlideAxis, true, true);
        }
    }
}