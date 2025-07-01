using System;
using UnityEngine;

namespace NiqonNO.Core.Utility.Attributes
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