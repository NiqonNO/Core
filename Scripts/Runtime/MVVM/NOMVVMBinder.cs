using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NiqonNO.Core.Utility.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core.MVVM
{
    public abstract class NOMVVMBinder<T> : MonoBehaviour
    {
        private const BindingFlags SourceBinding = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        private const BindingFlags TargetBinding = BindingFlags.Public | BindingFlags.Instance;

        [SerializeField, ValueDropdown(nameof(GetSourceOptions)),
         ValidateInput(nameof(ValidateSource))]
        private PropertyReference Source;

        [SerializeField, ValueDropdown(nameof(GetTargetOptions)),
         ValidateInput(nameof(ValidateTarget))]
        private PropertyReference Target;

        private IEnumerable GetSourceOptions() => GetPropertyOptions(GetComponentsInParent<Component>().Where(c => c is INOMVVMViewModel), SourceBinding, true);
        private IEnumerable GetTargetOptions() => GetPropertyOptions(GetComponents<Component>().Where(c => c != null && c != this), TargetBinding, false);
        private IEnumerable GetPropertyOptions(IEnumerable<Component> components, BindingFlags bindingFlags, bool requireAttribute)
        {
            return components
                .SelectMany(c => c.GetType().GetProperties(bindingFlags)
                    .Where(p => p.PropertyType == typeof(T) &&
                                (!requireAttribute || p.GetCustomAttribute<NOMVVMBindAttribute>() != null))
                    .Select(p => new ValueDropdownItem<PropertyReference>(
                        $"{c.GetType().Name}/{p.Name}",
                        new PropertyReference { Component = c, PropertyName = p.Name }
                    )));
        }
        
        private bool ValidateSource(ref string errorMessage) => ValidateProperty(Source, SourceBinding, true, ref errorMessage);
        private bool ValidateTarget(ref string errorMessage) => ValidateProperty(Target, SourceBinding, false, ref errorMessage);

        private bool ValidateProperty(PropertyReference reference, BindingFlags bindingFlags, bool isViewModel,
            ref string errorMessage)
        {
            if (reference.Component == null)
                return true;

            var property = GetPropertyInfo(reference, bindingFlags);
            if (property == null)
            {
                errorMessage = $"Could not find {reference.PropertyName} property in {reference.Component}";
                return false;
            }

            if (property.PropertyType != typeof(T))
            {
                errorMessage =
                    $"Property {reference.PropertyName} in {reference.Component} is of type {property.PropertyType}, expected {typeof(T)}";
                return false;
            }

            if (isViewModel)
            {
                if (property.GetCustomAttribute<NOMVVMBindAttribute>() == null)
                {
                    errorMessage =
                        $"Property {reference.PropertyName} in {reference.Component} lacks required {nameof(NOMVVMBindAttribute)}";
                    return false;
                }
                if (reference.Component is not INOMVVMViewModel)
                {
                    errorMessage =
                        $"Component {reference.Component} is not of type {nameof(INOMVVMViewModel)}";
                    return false;
                }
            }

            return true;
        }

        private void OnEnable()
        {
#if UNITY_EDITOR || DEBUG
            string err = "";
            Debug.Assert(ValidateSource(ref err), err);
            Debug.Assert(ValidateTarget(ref err), err);
#endif
            if (Source.Component is INOMVVMViewModel model)
            {
                model.RegisterOnViewModelChangeEvent(Bind);
            }
        }

        private void OnDisable()
        {
            if (Source.Component is INOMVVMViewModel model)
            {
                model.UnregisterOnViewModelChangeEvent(Bind);
            }
        }

        void Bind()
        {
            SetValue(Target, ProcessValue((T)GetValue(Source)));
        }

        protected virtual T ProcessValue(T rawValue) => rawValue;

        private PropertyInfo GetPropertyInfo(PropertyReference reference, BindingFlags bindingAttr)
        {
            if (reference.Component == null) return null;
            var property = reference.Component.GetType().GetProperty(reference.PropertyName, bindingAttr);
            return property;

        }
        private object GetValue(PropertyReference sourceReference)
        {
            var sourceProperty = GetPropertyInfo(sourceReference, SourceBinding);
            return sourceProperty.GetValue(sourceReference.Component);
        }

        private void SetValue(PropertyReference targetReference, object value)
        {
            var targetProperty = GetPropertyInfo(targetReference, TargetBinding);
            targetProperty.SetValue(targetReference.Component, value);
        }

        [Serializable]
        public struct PropertyReference
        {
            [ReadOnly] public Component Component;
            [ReadOnly] public string PropertyName;

            public override string ToString() =>  Component != null ? $"{Component.GetType().Name}/{PropertyName}" : "None";
        }
    }
}