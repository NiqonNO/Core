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

        public override int MaxPosition => Mathf.CeilToInt(ItemData.Count / CellsInRow);
        private float ViewHeight => (CellSize + Spacing) * MaxPosition - Spacing;
        
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
            Position = position * (MaxPosition - 1);
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
                Scrollbar.size = Mathf.Clamp(ViewportSize / CellContainerRect.size[1 - (int)ScrollDirection], 0.1f, 1);
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

                cell.SetContext(this);
                cell.Initialize();
                cell.Index = index;
                cell.SetData(ItemData.GetDataAt(index));
                cell.SetVisible(true);
            }

            for (int index = ItemData.Count; index < CellPool.Count; index++)
            {
                CellPool[index].SetVisible(false);
            }
        }
        void UpdateRect(bool forceRefresh = false)
        {
            var scrollAxis = 1 - (int)ScrollDirection;

            var slideArea = (CellContainerRect.size[scrollAxis] - ViewportSize);
            var pos = Position / (MaxPosition - 1);
            var offset = pos * slideArea;

            if (Mathf.Min(offset, slideArea - offset) > 1)
            {
                var rowPos = PaddingHead + Position * (CellSize + Spacing);
                offset = rowPos - (ViewportSize - CellSize) / 2f;
            }
            
            var anchoredPosition = CellContainer.anchoredPosition;
            anchoredPosition[scrollAxis] = offset;
            CellContainer.anchoredPosition = anchoredPosition;

            if (Scrollbar && UpdateScrollbar)
            {
                Scrollbar.SetValueWithoutNotify(offset / slideArea);
            }
            
            if (!forceRefresh) return;
            foreach (var t in CellPool)
            {
                if(t.IsVisible)
                    t.OnViewModelChange();
            }
        }

        public override void ScrollTo(float position, float duration, EasingFunction easingFunction,
            Action onComplete = null)
        {
            base.ScrollTo(TransformPosition(position), duration, easingFunction, onComplete);
            UpdateSelection(Mathf.RoundToInt(position));
        }

        public override void JumpTo(int index)
        {
            if (index < 0 || index > TotalCount - 1)
            {
                return;
            }

            UpdateSelection(index);
            Position = TransformPosition(index);
        }
        
        private float TransformPosition(float position)
        {
            return Mathf.FloorToInt(position / CellsInRow);
        }
    }
}