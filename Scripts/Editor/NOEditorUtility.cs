using System;
using System.Linq;
using Sirenix.Config;
using Sirenix.OdinInspector;
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
            if (!baseType.IsAssignableFrom(type))
                return includeBase ? type.Name.Split('`').First().SplitPascalCase() : "";
            var menuNamePath = type.Name.Split('`').First().SplitPascalCase();
            return type.BaseType.GetDerivedPath(baseType, includeBase) + "/" + menuNamePath;
        }

        public static string GetDerivedNicePath(this Type type, bool includeBase = false)
        {
            return type.GetDerivedPath(null, includeBase);
        }
        public static string GetDerivedNicePath(this Type type, Type baseType, bool includeBase = false)
        {
            if (type == null) return "";
            if (!baseType.IsAssignableFrom(type)) return includeBase ? type.GetNiceName() : "";
            var menuNamePath = type.GetNiceName();
            return type.BaseType.GetDerivedNicePath(baseType, includeBase) + "/" + menuNamePath;
        }
        public static string GetNiceName(this Type type)
        {
            var setting = TypeRegistryUserConfig.Instance.TryGetSettings(type);
            return setting == null ? type.Name.Split('`').First().SplitPascalCase() : setting.Name;
        }

        public static SdfIconType GetNiceIcon(this Type type)
        {
            if (type == null) return SdfIconType.None;
            var setting = TypeRegistryUserConfig.Instance.TryGetSettings(type);
            if (setting != null && setting.Icon != SdfIconType.None)
                return setting.Icon;
            return SdfIconType.PuzzleFill;
        }
    }
}