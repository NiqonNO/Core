using NiqonNO.Core.Utility.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace NiqonNO.Core.UI
{
    public class NOSelectorCellButton : NOSelectorCell
    {
        [SerializeField]
        private Color NeutralColor;
        [SerializeField]
        private Color SelectedColor;
        
        [SerializeField]
        Button Button;
        
        [NOMVVMBind] 
        protected bool Selected => Index == Context.SelectedIndex;
        [NOMVVMBind] 
        protected Color BackgroundColor => Selected ? SelectedColor : NeutralColor;
        
        public override void Initialize()
        {
            Button.onClick.AddListener(SelectCell);
        }

        private void SelectCell()
        {
            Context.ScrollTo(-((RectTransform)transform).anchoredPosition[1-(int)Context.ScrollDirection]);
        }
    }
}