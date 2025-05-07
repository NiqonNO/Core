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
        private float Spacing = 0f;
        
        protected override void Initialize()
        {
            ResizePool();
        }

        protected override void Relayout()
        {
            CellsInRow = 1;
            var cellRect = ((RectTransform)CellTemplate.transform).rect;

            var cellSizeParallel = cellRect.size[1 - (int)ScrollDirection];
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
                            var rectSizePerpendicular = CellContainer.rect.size[(int)ScrollDirection];
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

            JumpTo(SelectedIndex);
        }
        
        protected override void Refresh() => UpdateRect(true);
        protected override void OnUpdatePosition()
        {
            UpdateRect(true);
        }
        protected override void OnUpdateSelection() 
        {
            Refresh();
        }
        
        void ResizePool()
        {
            Debug.Assert(CellTemplate != null);
            Debug.Assert(CellContainer != null);

            for (var index = 0; index < ItemData.Count; index++)
            {
                var cell = Instantiate(CellTemplate, CellContainer);

                cell.SetContext(this);
                cell.Initialize();
                cell.Index = index;
                cell.SetData(ItemData.GetDataAt(index));
                cell.SetVisible(true);
                CellPool.Add(cell);
            }
        }
        void UpdateRect(bool forceRefresh = false)
        {
            if (!forceRefresh) return;
            foreach (var t in CellPool)
            {
                t.OnViewModelChange();
            }

            var anchoredPosition = CellContainer.anchoredPosition;
            anchoredPosition.y = Position / MaxPosition * ViewHeight;
            CellContainer.anchoredPosition = anchoredPosition;
        }

        public override void ScrollTo(float position, float duration, EasingFunction easingFunction,
            Action onComplete = null)
        {
            base.ScrollTo(TransformPosition(position), duration, easingFunction, onComplete);
            UpdateSelection(Mathf.RoundToInt(position));
        }

        private float TransformPosition(float position)
        {
            return Mathf.FloorToInt(position / CellsInRow);
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
    }
}