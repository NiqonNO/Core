using System;

namespace NiqonNO.Core.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class NOValueTextAreaAttribute : Attribute
    {
        public readonly int MinLines;
        public readonly int MaxLines;
        
        public NOValueTextAreaAttribute()
        {
            this.MinLines = 3;
            this.MaxLines = 3;
        }
        
        public NOValueTextAreaAttribute(int minLines, int maxLines)
        {
            this.MinLines = minLines;
            this.MaxLines = maxLines;
        }
    }
}