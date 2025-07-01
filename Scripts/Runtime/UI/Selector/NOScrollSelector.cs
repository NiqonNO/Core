using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NiqonNO.Core.UI
{
    public class NOScrollSelector : NOSelector
    {
        [SerializeField, Range(1e-2f, 1f)] 
        protected float CellInterval = 0.2f;

        [SerializeField, Range(0f, 1f)] 
        protected float ScrollOffset = 0.5f;

        [SerializeField] 
        protected bool Loop = false;
        
        [SerializeField] 
        private UnityEvent<INODataProvider> _OnHighlightedItemChanged = new();
        public UnityEvent<INODataProvider> OnHighlightedItemChanged
        {
            get => _OnHighlightedItemChanged;
            set => _OnHighlightedItemChanged = value;
        }

        public override int MaxPosition => TotalCount;
        readonly IList<NOSelectorCell> CellPool = new List<NOSelectorCell>();

        public int HighlightedIndex { get; private set; } = -1;

        protected override void Relayout()
        {
            base.Relayout();
            HandleCells();
        }
        protected override void OnUpdatePosition()
        {
            var position = Position;
            if(MovementType == MovementType.Elastic)
            {
                position = Mathf.Clamp(position, 0, MaxPosition - 1);
            }
            HighlightedIndex =  (int)CircularPosition(Mathf.RoundToInt(position));
            HandleCells(true);
            OnHighlightedItemChanged.Invoke(ItemData.GetGenericDataAt(HighlightedIndex));
        }
        protected override void OnUpdateSelection() 
        {
            HighlightedIndex = SelectedIndex;
            HandleCells(true);
        }
        
        protected override void PositionMovementStopped(float position)
        {
            int index = Mathf.RoundToInt(CircularPosition(position));
            UpdateSelection(index);
        }

        public void ScrollDown() => ScrollTo(HighlightedIndex - 1);
        public void ScrollUp() => ScrollTo(HighlightedIndex + 1);
        
        void HandleCells(bool forceRefresh = false)
        {
            var p = Position - ScrollOffset / CellInterval;
            var firstIndex = Mathf.CeilToInt(p);
            var firstPosition = (Mathf.Ceil(p) - p) * CellInterval;

            if (firstPosition + CellPool.Count * CellInterval < 1f)
            {
                ResizePool(firstPosition);
            }

            UpdateCells(firstPosition, firstIndex, forceRefresh);
        }
        void ResizePool(float firstPosition)
        {
            Debug.Assert(CellTemplate != null);
            Debug.Assert(CellContainer != null);

            var addCount = Mathf.CeilToInt((1f - firstPosition) / CellInterval) - CellPool.Count;
            for (var i = 0; i < addCount; i++)
            {
                var cell = Instantiate(CellTemplate, CellContainer);
                
                cell.Initialize(this);
                cell.SetVisible(false);
                CellPool.Add(cell);
            }
        }
        void UpdateCells(float firstPosition, int firstIndex, bool forceRefresh)
        {
            for (var i = 0; i < CellPool.Count; i++)
            {
                var index = firstIndex + i;
                var position = firstPosition + i * CellInterval;
                var cell = CellPool[CircularIndex(index, CellPool.Count)];

                if (Loop)
                {
                    index = CircularIndex(index, ItemData.Count);
                }

                if (index < 0 || index >= ItemData.Count || position > 1f)
                {
                    cell.SetVisible(false);
                    continue;
                }

                RefreshCell(cell, index);
                cell.UpdatePosition(position);
            }

            void RefreshCell(NOSelectorCell cell, int index)
            {
                if (cell.Index != index)
                {
                    cell.Index = index;
                    cell.SeCellData(ItemData.GetGenericDataAt(index));
                }
                else if (forceRefresh)
                {
                    cell.ForceRefresh();
                }

                if (!cell.IsVisible())
                {
                    cell.SetVisible(true);
                }
            }
        }

        protected int CircularIndex(int i, int count) => Mathf.RoundToInt(CircularPosition(i, count));
    }
}
