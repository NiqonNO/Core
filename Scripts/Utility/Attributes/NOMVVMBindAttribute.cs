using System;
using UnityEngine;

namespace NiqonNO.Core.Utility.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class NOMVVMBindAttribute : PropertyAttribute
    {
        public string Key;

        public NOMVVMBindAttribute(string key = null)
        {
            Key = key;
        }
    }
}