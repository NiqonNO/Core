using System;
using NiqonNO.Core.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NiqonNO.Core.UI
{
    public abstract class NOSelector : NOUIBehaviour, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler,
        IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement
    {
        protected readonly AutoScrollState AutoScroll = new AutoScrollState();
        static readonly EasingFunction DefaultEasingFunction = NOEasing.Get(Ease.OutCubic);
        
        [SerializeField] 
        private RectTransform _Viewport;
        public RectTransform Viewport
        {
            get => _Viewport;
            set => _Viewport = value;}
        
        [SerializeField, ChildGameObjectsOnly]
        private NOSelectorCell _CellTemplate;
        public NOSelectorCell CellTemplate {
            get => _CellTemplate;
            set => _CellTemplate = value;}

        [SerializeField] 
        NODataProviderCollectionBase _ItemData = default;
        public NODataProviderCollectionBase ItemData => _ItemData;
        
        [SerializeField] 
        ScrollDirection _ScrollDirection = ScrollDirection.Vertical;
        public ScrollDirection ScrollDirection => _ScrollDirection;

        [SerializeField] 
        MovementType _MovementType = MovementType.Elastic;
        public MovementType MovementType
        {
            get => _MovementType;
            set => _MovementType = value;
        }

        [SerializeField] 
        float _Elasticity = 0.1f;
        public float Elasticity
        {
            get => _Elasticity;
            set => _Elasticity = value;
        }

        [SerializeField] 
        float _ScrollSensitivity = 1f;
        public float ScrollSensitivity
        {
            get => _ScrollSensitivity;
            set => _ScrollSensitivity = value;
        }

        [SerializeField] 
        bool _Inertia = true;
        public bool Inertia
        {
            get => _Inertia;
            set => _Inertia = value;
        }

        [SerializeField] 
        float _DecelerationRate = 0.03f;
        public float DecelerationRate
        {
            get => _DecelerationRate;
            set => _DecelerationRate = value;
        }

        [SerializeField] 
        SnapConfig Snap = new SnapConfig
        {
            Enable = true,
            VelocityThreshold = 0.5f,
            Duration = 0.3f,
            Easing = Ease.InOutCubic
        };
        public bool SnapEnabled
        {
            get => Snap.Enable;
            set => Snap.Enable = value;
        }

        [SerializeField] 
        bool _Draggable = true;
        public bool Draggable
        {
            get => _Draggable;
            set => _Draggable = value;
        }

        [Space] 
        [SerializeField] 
        private UnityEvent<int> _OnItemSelected = new();
        public UnityEvent<int> OnItemSelected
        {
            get => _OnItemSelected;
            set => _OnItemSelected = value;
        }

        public float Position
        {
            get => CurrentPosition;
            set
            {
                AutoScroll.Reset();
                Velocity = 0f;
                Dragging = false;

                UpdatePosition(value);
            }
        }

        
        public bool LayoutReady { get; private set; }
        
        public int TotalCount => ItemData.Count;
        public abstract int MaxPosition { get; }
        public int SelectedIndex { get; private set; } = -1;
        public float ViewportSize => _ScrollDirection == ScrollDirection.Horizontal
            ? ViewportRect.size.x
            : ViewportRect.size.y;
        public float CellContainerSize => _ScrollDirection == ScrollDirection.Horizontal
            ? CellContainerRect.size.x
            : CellContainerRect.size.y;

        protected Rect ViewportRect;
        private RectTransform Cell;
        protected Rect CellRect;
        protected RectTransform CellContainer;
        protected Rect CellContainerRect;

        Vector2 BeginDragPointerPosition;
        float ScrollStartPosition;
        float PrevPosition;
        float CurrentPosition;

        bool Hold;
        bool Scrolling;
        bool Dragging;
        float Velocity;

        protected override void Start()
        {
            base.Start();
            Cell = ((RectTransform)CellTemplate.transform);
            CellRect = Cell.rect;
            CellContainer = (RectTransform)Cell.parent;
            
            CellTemplate.gameObject.SetActive(false);
            Initialize();
        }

        protected override void OnEnable() => CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        protected override void OnDisable() => CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

        public void Rebuild(CanvasUpdate executing) {}
        public void GraphicUpdateComplete() {}
        public void LayoutComplete()
        {
            LayoutReady = true;
            Relayout();
            JumpTo(0);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (!LayoutReady)
                return;
            Relayout();
        }

        public void SetData(NODataProvider newDataContainer)
        {
            if(newDataContainer is INODataProviderCollectionContainer newData)
                SetData(newData.DataCollection);
        }

        public void SetData(NODataProviderCollectionBase newData)
        {
            _ItemData = newData;
            Initialize();
            Relayout();
            JumpTo(0);
        }

        protected virtual void Initialize() {}

        protected virtual void Relayout()
        {
            ViewportRect = Viewport.rect;
            CellContainerRect = CellContainer.rect;
        }
        protected virtual void OnUpdatePosition() { }
        protected virtual void OnUpdateSelection() { }
        protected virtual void PositionMovementStopped(float position) { }
        
        void UpdatePosition(float position)
        {
            CurrentPosition = position;
            OnUpdatePosition();
        }
        protected void UpdateSelection(int index)
        {
            SelectedIndex = index;
            OnUpdateSelection();
            ItemData.Raise(ItemData.GetGenericDataAt(SelectedIndex));
            OnItemSelected.Invoke(SelectedIndex);
        }
        
        public virtual void ScrollTo(float position, Action onComplete = null) => ScrollTo(position, Snap.Duration, Snap.Easing, onComplete);
        public virtual void ScrollTo(float position, float duration, Action onComplete = null) => ScrollTo(position, duration, Ease.OutCubic, onComplete);
        public virtual void ScrollTo(float position, float duration, Ease easing, Action onComplete = null) => ScrollTo(position, duration, NOEasing.Get(easing), onComplete);
        public virtual void ScrollTo(float position, float duration, EasingFunction easingFunction, Action onComplete = null)
        {
            if (duration <= 0f)
            {
                Position = CircularPosition(position);
                onComplete?.Invoke();
                return;
            }

            AutoScroll.Reset();
            AutoScroll.Enable = true;
            AutoScroll.Duration = duration;
            AutoScroll.EasingFunction = easingFunction ?? DefaultEasingFunction;
            AutoScroll.StartTime = Time.unscaledTime;
            AutoScroll.EndPosition = CurrentPosition + CalculateMovementAmount(CurrentPosition, position);
            AutoScroll.OnComplete = onComplete;

            Velocity = 0f;
            ScrollStartPosition = CurrentPosition;

            PositionMovementStopped(AutoScroll.EndPosition);
        }
        public virtual void JumpTo(int index)
        {
            if (index < 0 || index > TotalCount - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            UpdateSelection(index);
            Position = index;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!_Draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            Hold = true;
            Velocity = 0f;
            AutoScroll.Reset();
        }
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!_Draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (Hold && Snap.Enable)
            {
                PositionMovementStopped(CurrentPosition);
                ScrollTo(Mathf.RoundToInt(CurrentPosition), Snap.Duration, Snap.Easing);
            }

            Hold = false;
        }
        public virtual void OnScroll(PointerEventData eventData)
        {
            if (!_Draggable)
            {
                return;
            }

            var delta = eventData.scrollDelta;

            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;
            var scrollDelta = _ScrollDirection == ScrollDirection.Horizontal
                ? Mathf.Abs(delta.y) > Mathf.Abs(delta.x)
                    ? delta.y
                    : delta.x
                : Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
                    ? delta.x
                    : delta.y;

            if (eventData.IsScrolling())
            {
                Scrolling = true;
            }

            var position = CurrentPosition + scrollDelta / ViewportSize * _ScrollSensitivity;
            if (_MovementType == MovementType.Clamped)
            {
                position += CalculateOffset(position);
            }

            if (AutoScroll.Enable)
            {
                AutoScroll.Reset();
            }

            UpdatePosition(position);
        }
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!_Draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            Hold = false;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Viewport,
                eventData.position,
                eventData.pressEventCamera,
                out BeginDragPointerPosition);

            ScrollStartPosition = CurrentPosition;
            Dragging = true;
            AutoScroll.Reset();
        }
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!_Draggable || eventData.button != PointerEventData.InputButton.Left || !Dragging)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    Viewport,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var dragPointerPosition))
            {
                return;
            }

            var pointerDelta = dragPointerPosition - BeginDragPointerPosition;
            var position = (_ScrollDirection == ScrollDirection.Horizontal ? -pointerDelta.x : pointerDelta.y)
                           / ViewportSize
                           + ScrollStartPosition;

            var offset = CalculateOffset(position);
            position += offset;

            if (_MovementType == MovementType.Elastic)
            {
                if (offset != 0f)
                {
                    position -= RubberDelta(offset, _ScrollSensitivity);
                }
            }

            UpdatePosition(position);
        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!_Draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            Dragging = false;
        }

        protected virtual void Update()
        {
            var deltaTime = Time.unscaledDeltaTime;
            var offset = CalculateOffset(CurrentPosition);

            if (AutoScroll.Enable)
            {
                HandleAutoScrollMovement(offset, deltaTime);
            }
            else if (!(Dragging || Scrolling) &&
                     (!Mathf.Approximately(offset, 0f) || !Mathf.Approximately(Velocity, 0f)))
            {
                HandleFreeMovement(offset, deltaTime);
            }

            if (!AutoScroll.Enable && (Dragging || Scrolling) && _Inertia)
            {
                var newVelocity = (CurrentPosition - PrevPosition) / deltaTime;
                Velocity = Mathf.Lerp(Velocity, newVelocity, deltaTime * 10f);
            }

            PrevPosition = CurrentPosition;
            Scrolling = false;
        }
        private void HandleAutoScrollMovement(float offset, float deltaTime)
        {
            var position = 0f;

            if (AutoScroll.Elastic)
            {
                position = Mathf.SmoothDamp(CurrentPosition, CurrentPosition + offset, ref Velocity,
                    _Elasticity, Mathf.Infinity, deltaTime);

                if (Mathf.Abs(Velocity) < 0.01f)
                {
                    position = Mathf.Clamp(Mathf.RoundToInt(position), 0, MaxPosition - 1);
                    Velocity = 0f;
                    AutoScroll.Complete();
                }
            }
            else
            {
                var alpha = Mathf.Clamp01((Time.unscaledTime - AutoScroll.StartTime) /
                                          Mathf.Max(AutoScroll.Duration, float.Epsilon));
                position = Mathf.LerpUnclamped(ScrollStartPosition, AutoScroll.EndPosition,
                    AutoScroll.EasingFunction(alpha));

                if (Mathf.Approximately(alpha, 1f))
                {
                    AutoScroll.Complete();
                }
            }

            UpdatePosition(position);
        }
        private void HandleFreeMovement(float offset, float deltaTime)
        {
            var position = CurrentPosition;

            if (_MovementType == MovementType.Elastic && !Mathf.Approximately(offset, 0f))
            {
                HandleFreeMovementElasticOffset(position);
            }
            else if (_Inertia)
            {
                HandleFreeMovementInertia(deltaTime, ref position);
            }
            else
            {
                Velocity = 0f;
            }

            if (Mathf.Approximately(Velocity, 0f)) return;
            
            if (_MovementType == MovementType.Clamped)
            {
                HandleFreeMovementClampedOffset(ref position);
            }

            UpdatePosition(position);
        }
        private void HandleFreeMovementElasticOffset(float position)
        {
            AutoScroll.Reset();
            AutoScroll.Enable = true;
            AutoScroll.Elastic = true;

            PositionMovementStopped(Mathf.Clamp(position, 0, MaxPosition - 1));
        }
        private void HandleFreeMovementInertia(float deltaTime, ref float position)
        {
            Velocity *= Mathf.Pow(_DecelerationRate, deltaTime);

            if (Mathf.Abs(Velocity) < 0.001f)
            {
                Velocity = 0f;
            }

            position += Velocity * deltaTime;

            if (!Snap.Enable || !(Mathf.Abs(Velocity) < Snap.VelocityThreshold)) return;
            
            ScrollTo(Mathf.RoundToInt(CurrentPosition), Snap.Duration, Snap.Easing);
        }
        private void HandleFreeMovementClampedOffset(ref float position)
        {
            float offset = CalculateOffset(position);
            position += offset;

            if (!Mathf.Approximately(position, 0f) && !Mathf.Approximately(position, MaxPosition - 1f)) return;
            
            Velocity = 0f;
            PositionMovementStopped(position);
        }

        float CalculateOffset(float position)
        {
            if (_MovementType == MovementType.Unrestricted)
            {
                return 0f;
            }

            if (position < 0f)
            {
                return -position;
            }

            if (position > MaxPosition - 1)
            {
                return MaxPosition - 1 - position;
            }

            return 0f;
        }
        float CalculateMovementAmount(float sourcePosition, float destPosition)
        {
            if (_MovementType != MovementType.Unrestricted)
            {
                return Mathf.Clamp(destPosition, 0, MaxPosition - 1) - sourcePosition;
            }

            var amount = CircularPosition(destPosition) - CircularPosition(sourcePosition);

            if (Mathf.Abs(amount) > MaxPosition * 0.5f)
            {
                amount = Mathf.Sign(-amount) * (MaxPosition - Mathf.Abs(amount));
            }

            return amount;
        }
        public SliderDirection GetMovementDirection(int sourceIndex, int destIndex)
        {
            var movementAmount = CalculateMovementAmount(sourceIndex, destIndex);
            return _ScrollDirection == ScrollDirection.Horizontal
                ? movementAmount > 0
                    ? SliderDirection.RightToLeft
                    : SliderDirection.LeftToRight
                : movementAmount > 0
                    ? SliderDirection.BottomToTop
                    : SliderDirection.TopToBottom;
        }

        float RubberDelta(float overStretching, float viewSize) => (1 - 1 / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1)) * viewSize * Mathf.Sign(overStretching);
        protected virtual float CircularPosition(float p) => CircularPosition(p, TotalCount);
        protected float CircularPosition(float p, int size) => size < 1 ? 0 : p < 0 ? size - 1 + (p + 1) % size : p % size;
        
        [Serializable]
        protected class SnapConfig
        {
            public bool Enable;
            public float VelocityThreshold;
            public float Duration;
            public Ease Easing;
        }
        protected class AutoScrollState
        {
            public bool Enable;
            public bool Elastic;
            public float Duration;
            public EasingFunction EasingFunction;
            public float StartTime;
            public float EndPosition;

            public Action OnComplete;

            public void Reset()
            {
                Enable = false;
                Elastic = false;
                Duration = 0f;
                StartTime = 0f;
                EasingFunction = DefaultEasingFunction;
                EndPosition = 0f;
                OnComplete = null;
            }

            public void Complete()
            {
                OnComplete?.Invoke();
                Reset();
            }
        }
    }
}
