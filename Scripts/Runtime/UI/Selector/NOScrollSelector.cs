using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NiqonNO.Core.UI
{
    public class NOScrollSelector : NOSelector
    {
        [SerializeField, Range(1e-2f, 1f)] protected float CellInterval = 0.2f;

        [SerializeField, Range(0f, 1f)] protected float ScrollOffset = 0.5f;

        [SerializeField] protected bool Loop = false;
        
        [SerializeField] 
        private UnityEvent<int> _OnHighlightedItemChanged = new();
        public UnityEvent<int> OnHighlightedItemChanged
        {
            get => _OnHighlightedItemChanged;
            set => _OnHighlightedItemChanged = value;
        }

        public override float MaxPosition => TotalCount;
        readonly IList<NOSelectorCell> pool = new List<NOSelectorCell>();

        public int HighlightedIndex { get; private set; } = -1;

        protected override void Relayout() => HandleCells();
        protected override void Refresh() => HandleCells(true);

        protected override void OnUpdatePosition()
        {
            HighlightedIndex =  (int)CircularPosition(Mathf.RoundToInt(CurrentPosition));
            HandleCells(true);
            OnHighlightedItemChanged.Invoke(HighlightedIndex);
        }

        protected override void OnUpdateSelection() 
        {
            HighlightedIndex = SelectedIndex;
            Refresh();
        }

        public void ScrollDown() => ScrollTo(HighlightedIndex - 1);
        public void ScrollUp() => ScrollTo(HighlightedIndex + 1);
        
        void HandleCells(bool forceRefresh = false)
        {
            var p = Position - ScrollOffset / CellInterval;
            var firstIndex = Mathf.CeilToInt(p);
            var firstPosition = (Mathf.Ceil(p) - p) * CellInterval;

            if (firstPosition + pool.Count * CellInterval < 1f)
            {
                ResizePool(firstPosition);
            }

            UpdateCells(firstPosition, firstIndex, forceRefresh);
        }

        void ResizePool(float firstPosition)
        {
            Debug.Assert(CellTemplate != null);
            Debug.Assert(CellContainer != null);

            var addCount = Mathf.CeilToInt((1f - firstPosition) / CellInterval) - pool.Count;
            for (var i = 0; i < addCount; i++)
            {
                var cell = Instantiate(CellTemplate, CellContainer);

                cell.SetContext(this);
                cell.Initialize();
                cell.SetVisible(false);
                pool.Add(cell);
            }
        }

        void UpdateCells(float firstPosition, int firstIndex, bool forceRefresh)
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var index = firstIndex + i;
                var position = firstPosition + i * CellInterval;
                var cell = pool[CircularIndex(index, pool.Count)];

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
                    cell.SetData(ItemData.GetDataAt(index));
                }
                else if (forceRefresh)
                {
                    cell.OnViewModelChange();
                }

                if (!cell.IsVisible)
                {
                    cell.SetVisible(true);
                }
            }
        }

        protected int CircularIndex(int i, int count) => Mathf.RoundToInt(CircularPosition(i, count));
        
#if UNITY_EDITOR
        bool cachedLoop;
        float cachedCellInterval, cachedScrollOffset;

        void LateUpdate()
        {
            if (cachedLoop != Loop ||
                cachedCellInterval != CellInterval ||
                cachedScrollOffset != ScrollOffset)
            {
                cachedLoop = Loop;
                cachedCellInterval = CellInterval;
                cachedScrollOffset = ScrollOffset;

                HandleCells();
            }
        }
#endif
    }
}
