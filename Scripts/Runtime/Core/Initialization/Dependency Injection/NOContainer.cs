using System;
using System.Collections.Generic;
using NiqonNO.Core.Scene;

namespace NiqonNO.Core
{
    public partial class NOContainer
    {
        private readonly Dictionary<Type, object> Registrations = new();
        private readonly NOContainer Parent;
        public readonly string MyScope = string.Empty;

        public NOContainer()
        {
            
        }
        public NOContainer(string myScope)
        {
            MyScope = myScope;
            if(NOSceneDependencyData.TryGetSceneParent(myScope, out var parent) &&
               Registry.TryGetContainer(parent, out var parentContainer))
                Parent = parentContainer;
        }
        
        public void ActivateScope(INOContext context)
        {
            Registrations[typeof(INOContext)] = context;
            Registry.RegisterContainer(this);
        }

        public void DeactivateScope()
        {
            Registrations.Remove(typeof(INOContext));
            Registry.UnregisterContainer(this);
        }


        public void RegisterService(INOManager manager)
        {
            var interfaces = manager.GetType().GetInterfaces();
            foreach (var iface in interfaces)
            {
                if (iface == typeof(INOService) ||
                    !typeof(INOService).IsAssignableFrom(iface)) continue;
                Registrations[iface] = manager;
            }
            Registry.EnqueueManagerForInitialization(MyScope, manager);
        }

        public void UnregisterService(INOManager manager)
        {
            var interfaces = manager.GetType().GetInterfaces();
            foreach (var iface in interfaces)
            {
                if (iface == typeof(INOService) ||
                    !typeof(INOService).IsAssignableFrom(iface)) continue;
                Registrations.Remove(iface);
            }
        }

        public void Inject(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            
            var fields = GetInjectableFields(target.GetType());
            foreach (var field in fields)
            {
                var service = Resolve(field.FieldType);
                field.SetValue(target, service);
            }
        }

        private T Resolve<T>() where T : INOService => (T)Resolve(typeof(T));
        private object Resolve(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (Registrations.TryGetValue(serviceType, out var service))
                return service;
            if(Parent == null)
                throw new InvalidOperationException($"Service of type {serviceType.FullName} is not registered.");
            return Parent.Resolve(serviceType);
        }
    }
}