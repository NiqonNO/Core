using System;
using System.Linq;
using Sirenix.Utilities;

namespace NiqonNO.Core.Editor
{
    public static class NOEditorUtility
    {
        public static string GetDerivedPath(this Type type, bool includeBase = false)
        {
            return type.GetDerivedPath(null, includeBase);
        }
        public static string GetDerivedPath(this Type type, Type baseType, bool includeBase = false)
        {
            if (type == null) return "";
            if (type == baseType) return includeBase ? type.Name.Split('`').First().SplitPascalCase() : "";
            var menuNamePath = type.Name.Split('`').First().SplitPascalCase();
            return type.BaseType.GetDerivedPath(baseType, includeBase) + "/" + menuNamePath;
        }
    }
}