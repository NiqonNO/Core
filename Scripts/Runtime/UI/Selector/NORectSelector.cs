using System;
using System.Collections.Generic;
using NiqonNO.Core.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace NiqonNO.Core.UI
{
    public class NORectSelector : NOSelector
    {
        [SerializeField] 
        Scrollbar _Scrollbar = default;
        public Scrollbar Scrollbar => _Scrollbar;
        
        [SerializeField] 
        LayoutGroup _Layout = default;
        public LayoutGroup Layout => _Layout;

        public override int MaxPosition => (CellContainerSize <= ViewportSize) ? 1 : Mathf.CeilToInt(ItemData.Count / CellsInRow);
        private int MaxMinusOne => Mathf.Max(1, MaxPosition - 1);
        
        readonly IList<NOSelectorCell> CellPool = new List<NOSelectorCell>();

        private float CellSize;
        private float CellsInRow;
        private float Spacing;
        private float PaddingHead;

        private bool UpdateScrollbar = true;
        
        protected override void Initialize()
        {
            ResizePool();
            
            if (Scrollbar)
            {
                Scrollbar.onValueChanged.AddListener(OnScrollbar);
            }
        }

        private void OnScrollbar(float position)
        {
            UpdateScrollbar = false;
            Position = position * MaxMinusOne;
            UpdateScrollbar = true;
        }

        protected override void Relayout()
        {
            base.Relayout();
            CellsInRow = 1;
            var cellSizeParallel = CellRect.size[1 - (int)ScrollDirection];
            var spacingParallel = 0.0f;
            
            switch (Layout)
            {
                case GridLayoutGroup gridLayout:
                {
                    switch (gridLayout.constraint)
                    {
                        case GridLayoutGroup.Constraint.FixedColumnCount when ScrollDirection == ScrollDirection.Vertical:
                        case GridLayoutGroup.Constraint.FixedRowCount when ScrollDirection == ScrollDirection.Horizontal:
                            CellsInRow = Mathf.Max(1,gridLayout.constraintCount);
                            break;
                        case GridLayoutGroup.Constraint.Flexible:
                        default:
                            var rectSizePerpendicular = CellContainerRect.size[(int)ScrollDirection];
                            var paddingPerpendicular = ScrollDirection == ScrollDirection.Vertical ? Layout.padding.horizontal : Layout.padding.vertical;
                            var spacingPerpendicular = gridLayout.spacing[(int)ScrollDirection];
                            var cellSizePerpendicular = gridLayout.cellSize[(int)ScrollDirection];
                            spacingParallel = gridLayout.spacing[1 - (int)ScrollDirection];
                            cellSizeParallel = gridLayout.cellSize[1 - (int)ScrollDirection];
                            CellsInRow = Mathf.Max(1, Mathf.Floor((rectSizePerpendicular + spacingPerpendicular - paddingPerpendicular) / (cellSizePerpendicular + spacingPerpendicular)));
                            break;
                    }
                    break;
                }
                case HorizontalOrVerticalLayoutGroup hvLayout:
                {
                    spacingParallel = hvLayout.spacing;
                    break;
                }
            }
            
            CellSize = cellSizeParallel;
            Spacing = spacingParallel;
            PaddingHead = ScrollDirection == ScrollDirection.Horizontal ? Layout.padding.right : Layout.padding.top;
            
            if (Scrollbar)
            {
                Scrollbar.size = Mathf.Clamp(ViewportSize / CellContainerSize, 0.1f, 1);
            }

            JumpTo(SelectedIndex);
        }
        
        protected override void OnUpdatePosition()
        {
            UpdateRect();
        }
        protected override void OnUpdateSelection() 
        {
            UpdateRect(true);
        }

        protected override void Update()
        {
            base.Update();
            UpdateSliderVisibility();
        }

        void ResizePool()
        {
            Debug.Assert(CellTemplate != null);
            Debug.Assert(CellContainer != null);

            for (var index = 0; index < ItemData.Count; index++)
            {
                NOSelectorCell cell;
                if(CellPool.Count <= index)
                {
                    cell = Instantiate(CellTemplate, CellContainer);
                    CellPool.Add(cell);
                }
                else
                    cell = CellPool[index];


                cell.Initialize(this);
                cell.Index = index;
                cell.SeCellData(ItemData.GetDataAt(index));
                cell.SetVisible(true);
            }

            for (int index = ItemData.Count; index < CellPool.Count; index++)
            {
                CellPool[index].SetVisible(false);
            }
        }

        void UpdateSliderVisibility()
        {
            bool shouldShowScrollbar = CellContainerSize > ViewportSize;
            if (Scrollbar.gameObject.activeSelf != shouldShowScrollbar)
            {
                Scrollbar.gameObject.SetActive(shouldShowScrollbar);
            }
        }
        
        void UpdateRect(bool forceRefresh = false)
        {
            var scrollAxis = 1 - (int)ScrollDirection;
            var slideArea = Mathf.Max(10,CellContainerSize - ViewportSize);
            var pos = Position / MaxMinusOne;
            var offset = pos * slideArea;

            var anchoredPosition = CellContainer.anchoredPosition;
            anchoredPosition[scrollAxis] = offset;
            CellContainer.anchoredPosition = anchoredPosition;

            if (UpdateScrollbar && Scrollbar && Scrollbar.gameObject.activeSelf)
            {
                var sliderVal = offset / slideArea;
                var normalizedVal = Mathf.Clamp01(sliderVal);
                Scrollbar.SetValueWithoutNotify(normalizedVal);
                Scrollbar.size = Mathf.Clamp(ViewportSize / CellContainerSize - Mathf.Abs(sliderVal - normalizedVal) * Elasticity, 0.1f, 1);
            }

            if (!forceRefresh) return;
            foreach (var t in CellPool)
            {
                if (t.IsVisible())
                    t.ForceRefresh();
            }
        }

        public override void ScrollTo(float position, float duration, EasingFunction easingFunction,
            Action onComplete = null)
        {
            base.ScrollTo(CenterOnPosition(position), duration, easingFunction, onComplete);
            UpdateSelection(Mathf.RoundToInt(position));
        }

        public override void JumpTo(int index)
        {
            if (index < 0 || index > TotalCount - 1)
            {
                return;
            }

            UpdateSelection(index);
            Position = CenterOnPosition(index);
        }
        
        private float CenterOnPosition(float position)
        {
            float rowIndex = Mathf.Floor(position / CellsInRow);
            float cellOffset = rowIndex * (CellSize + Spacing);
            float targetPosition = PaddingHead + cellOffset;

            float scrollableRange = CellContainerSize - ViewportSize;
            float centeredOffset = targetPosition - (ViewportSize - CellSize) * 0.5f;

            float normalized = Mathf.Clamp01(centeredOffset / scrollableRange);
            return normalized * MaxMinusOne;
        }
    }
}