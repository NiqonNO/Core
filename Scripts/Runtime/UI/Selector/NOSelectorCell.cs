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

        public virtual void SeCellData(INODataProvider data) => Data = data;
        
        public virtual bool IsVisible() => gameObject.activeSelf;
        public virtual void SetVisible(bool visible) => gameObject.SetActive(visible);

        public abstract void ForceRefresh();
        
        public virtual void UpdatePosition(float position)
        {
            CurrentPosition = position;
        }
    }
}