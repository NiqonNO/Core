using System;

namespace NiqonNO.Core.Utility.Attributes
{
    public class NOStringValueMultilineAttribute : Attribute
    {
        public readonly int minLines;
        public readonly int maxLines;
        
        public NOStringValueMultilineAttribute()
        {
            this.minLines = 3;
            this.maxLines = 3;
        }
        
        public NOStringValueMultilineAttribute(int minLines, int maxLines)
        {
            this.minLines = minLines;
            this.maxLines = maxLines;
        }
    }
}