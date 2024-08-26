using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NiqonNO.Core.Utility.Attributes;
using Sirenix.Config;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace NiqonNO.Core.Editor.Drawers.AttributeDrawers
{
  public class NOComponentListAttributeDrawer<T1, T2> : OdinAttributeDrawer<NOComponentListAttribute, T1>
    where T1 : IList<T2> where T2 : class
  {
    private IEnumerable<Type>  ValidClassList;

    protected override void Initialize()
    {
      ValidClassList = AssemblyUtilities.GetTypes(AssemblyCategory.All)
          .Where(t =>
            t.IsClass &&
            !t.IsAbstract &&
            !t.IsGenericType &&
            typeof(T2).IsAssignableFrom(t) &&
            (Attribute.IncludeBaseClass || t != typeof(T2)));
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
      SirenixEditorGUI.BeginBox();
      CallNextDrawer(label);

      var selectedEnumerable = 
        OdinSelector<Type>.DrawSelectorDropdown(GUIContent.none, Attribute.AddButtonText, ShowSelector, 
          new GUIStyle(GUI.skin.GetStyle("Button")));

      SirenixEditorGUI.EndBox();
      if (selectedEnumerable == null) return;
      var selectedItems = selectedEnumerable.ToList();
      if (!selectedItems.Any()) return;
      foreach(var selected in selectedItems)
      {
        ValueEntry.SmartValue.Add((T2)Activator.CreateInstance(selected));
      }
    }

    private OdinSelector<Type> ShowSelector(Rect rect)
    {
      GenericSelector<Type> selector = CreateSelector();
      rect.x = (int)rect.x;
      rect.y = (int)rect.y;
      rect.width = (int)rect.width;
      rect.height = (int)rect.height;
      selector.ShowInPopup(rect.AlignCenterX(250));
      return selector;
    }

    private GenericSelector<Type> CreateSelector()
    {
      var valueDropdownItems = ValidClassList ?? Enumerable.Empty<Type>();

      GenericSelector<Type> selector = new GenericSelector<Type>(null, false,
        valueDropdownItems.Select(x => new GenericSelectorItem<Type>(x.GetDerivedPath(typeof(T2), Attribute.IncludeBaseClass), x)));
      
      selector.EnableSingleClickToSelect();
      selector.SelectionTree.EnumerateTree().ForEach(x =>
      {
        var valueType = (Type)x.Value;
        var setting = TypeRegistryUserConfig.Instance.TryGetSettings(valueType);
        if (setting != null && setting.Icon != SdfIconType.None)
          x.AddIcon(setting.Icon);
        else
          x.AddIcon(SdfIconType.PuzzleFill);

        x.IsEnabled = !Property.ValueEntry.WeakValues.Cast<IEnumerable>()
          .SelectMany(e => e.Cast<T2>())
          .Any(c => valueType.IsInstanceOfType(c) || c.GetType().IsAssignableFrom(valueType));
        x.SdfIconColor = x.IsEnabled ? Color.white : Color.grey;
      });
      selector.SelectionTree.Config.DrawSearchToolbar = true;
      selector.FlattenedTree = true;
      IEnumerable<Type> selection = Enumerable.Empty<Type>();
      selector.SetSelection(selection);
      selector.SelectionTree.EnumerateTree(x => x.Toggled = true);
      return selector;
    }
  }
}