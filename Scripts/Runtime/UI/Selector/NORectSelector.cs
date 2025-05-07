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
        
        readonly IList<NOSelectorCell> CellPool = new List<NOSelectorCell>();

        private float CellSize;
        private float CellsInRow;
        private float CellInterval;
        private float ScrollOffset;
        private float Spacing = 0f;
        
        protected override void Initialize()
        {
            ResizePool();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            
            CellsInRow = 1;
            var rect = CellContainer.rect;
            var cellRect = ((RectTransform)CellTemplate.transform).rect;
            
            var rectSizeParallel = rect.size[1 - (int)ScrollDirection];
            var cellSizeParallel = cellRect.size[1 - (int)ScrollDirection];
            var spacingParallel = 0.0f;
            
            switch (Layout)
            {
                case GridLayoutGroup gridLayout:
                {
                    var rectSizePerpendicular = rect.size[(int)ScrollDirection];
                    var paddingPerpendicular = ScrollDirection == ScrollDirection.Vertical ? Layout.padding.horizontal : Layout.padding.vertical;
                    var spacingPerpendicular = gridLayout.spacing[(int)ScrollDirection];
                    var cellSizePerpendicular = gridLayout.cellSize[(int)ScrollDirection];
                    cellSizeParallel = gridLayout.cellSize[1 - (int)ScrollDirection];
                    CellsInRow = Mathf.Max(1, Mathf.Floor((rectSizePerpendicular + spacingPerpendicular - paddingPerpendicular) / (cellSizePerpendicular + spacingPerpendicular)));
                    break;
                }
                case HorizontalOrVerticalLayoutGroup hvLayout:
                    spacingParallel = hvLayout.spacing;
                    break;
            }
            
            CellInterval = (cellSizeParallel + spacingParallel) / rectSizeParallel;
            ScrollOffset = CellInterval;
            CellSize = cellSizeParallel;
            Spacing = spacingParallel;
        }

        protected override void Relayout() => UpdateRect();
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
            anchoredPosition.y = Position / MaxPosition * CellContainer.rect.height;
            CellContainer.anchoredPosition = anchoredPosition;
        }
    }
}