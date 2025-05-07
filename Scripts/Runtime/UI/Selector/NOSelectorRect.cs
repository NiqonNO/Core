using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NiqonNO.Core.UI
{
    public class NOSelectorRect : NOSelector
    {
        [SerializeField] Scrollbar scrollbar = default;
        public Scrollbar Scrollbar => scrollbar;
        
        public override float MaxPosition => Mathf.Max(0, CellContainer.rect.size[1-(int)ScrollDirection] - Viewport.rect.size[1-(int)ScrollDirection]);
        readonly IList<NOSelectorCell> pool = new List<NOSelectorCell>();

        protected override void Relayout() => HandleCells();
        protected override void Refresh() => HandleCells(true);

        protected override void OnUpdatePosition()
        {
            HandleCells(true);
            HandleRect();
        }

        protected override void OnUpdateSelection() 
        {
            Refresh();
        }
        
        void HandleCells(bool forceRefresh = false)
        {
            if (ItemData.Count > pool.Count)
            {
                ResizePool();
            }

            UpdateCells(forceRefresh);
        }
        void ResizePool()
        {
            Debug.Assert(CellTemplate != null);
            Debug.Assert(CellContainer != null);

            var addCount = ItemData.Count - pool.Count;
            for (var i = 0; i < addCount; i++)
            {
                var cell = Instantiate(CellTemplate, CellContainer);

                cell.SetContext(this);
                cell.Initialize();
                cell.SetVisible(true);
                pool.Add(cell);
            }
        }
        void UpdateCells(bool forceRefresh)
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var index = i;
                var cell = pool[index];

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
        void HandleRect()
        {
            Vector2 position = CellContainer.anchoredPosition;
            position[1 - (int)ScrollDirection] = CurrentPosition;
            CellContainer.anchoredPosition = position;
        }
    }
}