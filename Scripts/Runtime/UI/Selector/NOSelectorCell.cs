using NiqonNO.Core.Utility.Attributes;

namespace NiqonNO.Core.UI
{
    public class NOSelectorCell : NODataProviderViewModel<NOSelector>
    {
        [NOMVVMBind] 
        protected string ItemIndex => $"{Index + 1}/{Context.TotalCount}";
        
        public int Index { get; set; } = -1;
        public virtual bool IsVisible => gameObject.activeSelf;
        
        float CurrentPosition = 0;

        void OnEnable() => UpdatePosition(CurrentPosition);
        
        public virtual void Initialize() { }
        
        public virtual void SetVisible(bool visible) => gameObject.SetActive(visible);
        
        public virtual void UpdatePosition(float position)
        {
            CurrentPosition = position;
        }
    }
}