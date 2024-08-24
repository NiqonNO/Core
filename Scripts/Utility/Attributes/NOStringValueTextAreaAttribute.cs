using System;

namespace NiqonNO.Core.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class NOStringValueTextAreaAttribute : Attribute
    {
        public readonly int minLines;
        public readonly int maxLines;
        
        public NOStringValueTextAreaAttribute()
        {
            this.minLines = 3;
            this.maxLines = 3;
        }
        
        public NOStringValueTextAreaAttribute(int minLines, int maxLines)
        {
            this.minLines = minLines;
            this.maxLines = maxLines;
        }
    }
}