using UnityEngine;

namespace NiqonNO.Core.UI
{
    public class NOSelectorCellAnimated : NOSelectorCell
    {
        private static readonly int Scroll = Animator.StringToHash("scroll");
        
        [SerializeField] 
        Animator Animator;
        
        public override void UpdatePosition(float position)
        {
            base.UpdatePosition(position);
                
            if (Animator.isActiveAndEnabled)
            {
                Animator.Play(Scroll, -1, position);
            }

            Animator.speed = 0;
        }
    }
}
