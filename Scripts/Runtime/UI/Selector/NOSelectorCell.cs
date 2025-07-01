using UnityEngine;

namespace NiqonNO.Core.UI
{
    public abstract class NOSelectorCell : MonoBehaviour
    {
        protected abstract INODataProvider Data { get; set; }
        public int Index { get; set; } = -1;
        
        float CurrentPosition = 0;

        void OnEnable() => UpdatePosition(CurrentPosition);
        
        public abstract void Initialize(NOSelector owner);

        public void SeCellData(INODataProvider data) => Data = data;
        
        public abstract bool IsVisible();
        public abstract void SetVisible(bool visible);
        public virtual void ForceRefresh() {}
        
        public virtual void UpdatePosition(float position)
        {
            CurrentPosition = position;
        }
    }
}