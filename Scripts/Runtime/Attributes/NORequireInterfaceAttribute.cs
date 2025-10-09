using System;

namespace NiqonNO.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class NORequireInterfaceAttribute : Attribute
    {
        public System.Type requiredType { get; private set; }

        public NORequireInterfaceAttribute(System.Type type)
        {
            this.requiredType = type;
        }
    }
}