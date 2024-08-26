using System;
using Sirenix.OdinInspector;

namespace NiqonNO.Core.Utility.Attributes
{
    [IncludeMyAttributes]
    [PolymorphicDrawerSettingsProxy(ShowBaseType = false, ReadOnlyIfNotNullReference = true)]
    [ListDrawerSettings(ShowFoldout = false, HideAddButton = true, DraggableItems = false)]
    [DisableContextMenu(true, true)]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NOComponentListAttribute : Attribute
    {
        public readonly string AddButtonText;
        public readonly bool IncludeBaseClass;

        public NOComponentListAttribute(bool includeBaseClass = false) : this("Add Component", includeBaseClass)
        { }

        public NOComponentListAttribute(string addButtonText, bool includeBaseClass = false)
        {
            AddButtonText = addButtonText;
            IncludeBaseClass = includeBaseClass;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    internal class PolymorphicDrawerSettingsProxyAttribute : PolymorphicDrawerSettingsAttribute { }
}